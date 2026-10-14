using System;
using RPGFramework.Audio;
using RPGFramework.Audio.Music;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Store;
using RPGFramework.Field.SharedTypes.Constants;
using RPGFramework.Field.SharedTypes.Stores;
using RPGFramework.Menu.SharedTypes;
using RPGFramework.Menu.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes.Stores;

namespace RPGFramework.ModuleRouter
{
    /// <summary>
    /// The framework's own routes between its modules: the menu over the field and closing back to it, a battle replacing
    /// the field and returning to it, a lost battle going to the game-over field unless the battle mode allows the loss,
    /// a game over going back to the title, and a playthrough begun or loaded from the menu. As a battle begins, the
    /// music playing is paused for the field to resume and the battle music started; leaving a playthrough or heading
    /// to a game over fades the music out. A game derives from it for routes of its own, overriding <see cref="Route" />
    /// and passing the rest to the base.
    /// </summary>
    public class DefaultModuleRouter : IModuleRouter
    {
        private const float MUSIC_FADE_OUT_SECONDS = 2f;

        private readonly ICurrentModuleStore       m_CurrentModuleStore;
        private readonly IBattleArgsStore          m_BattleArgsStore;
        private readonly IBattleModeStore          m_BattleModeStore;
        private readonly IBattleCompleteStateStore m_BattleCompleteStateStore;
        private readonly IBattleMusicStore         m_BattleMusicStore;
        private readonly IPausedMusicStore         m_PausedMusicStore;
        private readonly IFieldArgsStore           m_FieldArgsStore;
        private readonly IGameOverFieldStore       m_GameOverFieldStore;
        private readonly IMenuArgsStore            m_MenuArgsStore;
        private readonly IMusicPlayer              m_MusicPlayer;

        // Where a battle goes back to: the module it replaced.
        private byte m_ReturnModuleId;

        public DefaultModuleRouter(ICurrentModuleStore       currentModuleStore,
                                   IBattleArgsStore          battleArgsStore,
                                   IBattleModeStore          battleModeStore,
                                   IBattleCompleteStateStore battleCompleteStateStore,
                                   IBattleMusicStore         battleMusicStore,
                                   IPausedMusicStore         pausedMusicStore,
                                   IFieldArgsStore           fieldArgsStore,
                                   IGameOverFieldStore       gameOverFieldStore,
                                   IMenuArgsStore            menuArgsStore,
                                   IMusicPlayer              musicPlayer)
        {
            m_CurrentModuleStore       = currentModuleStore;
            m_BattleArgsStore          = battleArgsStore;
            m_BattleModeStore          = battleModeStore;
            m_BattleCompleteStateStore = battleCompleteStateStore;
            m_BattleMusicStore         = battleMusicStore;
            m_PausedMusicStore         = pausedMusicStore;
            m_FieldArgsStore           = fieldArgsStore;
            m_GameOverFieldStore       = gameOverFieldStore;
            m_MenuArgsStore            = menuArgsStore;
            m_MusicPlayer              = musicPlayer;
        }

        ModuleChange IModuleRouter.Route(byte moduleId, byte outcome)
        {
            ModuleChange change = Route(moduleId, outcome);

            return change;
        }

        /// <summary>
        /// What follows <paramref name="outcome" /> from the module on top, <paramref name="moduleId" />. Throws for an
        /// outcome with no route.
        /// </summary>
        protected virtual ModuleChange Route(byte moduleId, byte outcome)
        {
            ModuleChange change;

            switch (moduleId)
            {
                case FieldConstants.MODULE_ID:
                    change = RouteFromField(outcome);
                    break;
                case MenuConstants.MODULE_ID:
                    change = RouteFromMenu(outcome);
                    break;
                case BattleConstants.MODULE_ID:
                    change = RouteFromBattle(outcome);
                    break;
                default:
                    throw NoRoute(moduleId, outcome);
            }

            return change;
        }

        private ModuleChange RouteFromField(byte outcome)
        {
            ModuleChange change;

            switch (outcome)
            {
                case FieldOutcomes.OPEN_MENU:
                    change = ModuleChange.Over(MenuConstants.MODULE_ID);
                    break;
                case FieldOutcomes.START_BATTLE:
                    m_ReturnModuleId = FieldConstants.MODULE_ID;
                    StartBattleMusic();
                    change = ModuleChange.Replace(BattleConstants.MODULE_ID);
                    break;
                case FieldOutcomes.GAME_OVER:
                    FadeMusicOut();
                    m_MenuArgsStore.Set(new MenuArgs((byte)MenuType.Begin));
                    change = ModuleChange.Clear(MenuConstants.MODULE_ID);
                    break;
                default:
                    throw NoRoute(FieldConstants.MODULE_ID, outcome);
            }

            return change;
        }

        private ModuleChange RouteFromMenu(byte outcome)
        {
            ModuleChange change;

            switch (outcome)
            {
                case MenuOutcomes.CLOSED:
                    change = ModuleChange.Close();
                    break;
                case MenuOutcomes.PLAYTHROUGH:
                    FadeMusicOut();
                    change = ModuleChange.Clear(m_CurrentModuleStore.ModuleId);
                    break;
                default:
                    throw NoRoute(MenuConstants.MODULE_ID, outcome);
            }

            return change;
        }

        private ModuleChange RouteFromBattle(byte outcome)
        {
            if (outcome != BattleOutcomes.FINISHED)
            {
                throw NoRoute(BattleConstants.MODULE_ID, outcome);
            }

            ModuleChange change;

            if (IsGameOver())
            {
                FadeMusicOut();
                m_FieldArgsStore.Set(m_GameOverFieldStore.Args);
                change = ModuleChange.Replace(FieldConstants.MODULE_ID);
            }
            else
            {
                change = ModuleChange.Replace(m_ReturnModuleId, ModuleEntries.RETURN);
            }

            return change;
        }

        // A defeat, or a countdown running out, ends the game unless the battle mode says otherwise.
        private bool IsGameOver()
        {
            BattleCompleteState state = m_BattleCompleteStateStore.State;
            BattleFlags         mode  = m_BattleModeStore.Mode;

            bool gameOver = state switch
                            {
                                BattleCompleteState.DEFEAT      => (mode & BattleFlags.DISABLE_GAME_OVER_ON_DEFEAT) == 0,
                                BattleCompleteState.OUT_OF_TIME => (mode & BattleFlags.DISABLE_GAME_OVER_ON_TIME_OUT) == 0,
                                _                               => false
                            };

            return gameOver;
        }

        // While the module the battle replaces is still on screen, as the battle's transition begins.
        private void StartBattleMusic()
        {
            if (m_BattleArgsStore.Args.HasFlag(BattleFlags.USE_CURRENT_MUSIC))
            {
                return;
            }

            MusicSnapshot snapshot = m_MusicPlayer.Pause();
            m_PausedMusicStore.Set(snapshot);

            ulong battleMusic = m_BattleMusicStore.BattleMusic;

            if (battleMusic != 0)
            {
                m_MusicPlayer.PlayAsync(battleMusic).FireAndForget();
            }
        }

        // Under the screen fading out, as the next module brings its own music.
        private void FadeMusicOut()
        {
            m_MusicPlayer.StopAsync(MUSIC_FADE_OUT_SECONDS).FireAndForget();
        }

        private static InvalidOperationException NoRoute(byte moduleId, byte outcome)
        {
            InvalidOperationException exception = new InvalidOperationException($"{nameof(DefaultModuleRouter)}::{nameof(Route)} No route from module [{moduleId}] for outcome [{outcome}]");

            return exception;
        }
    }
}