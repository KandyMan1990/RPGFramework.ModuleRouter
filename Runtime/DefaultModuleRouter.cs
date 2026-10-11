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
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter
{
    /// <summary>
    /// The framework's own routes between its modules: the menu over the field and closing back to it, a battle replacing
    /// the field and returning to it, and a playthrough begun or loaded from the menu. As a battle begins, the music
    /// playing is paused for the field to resume and the battle music started. A game derives from it for routes of its
    /// own, overriding <see cref="Route" /> and passing the rest to the base.
    /// </summary>
    public class DefaultModuleRouter : IModuleRouter
    {
        private readonly ICurrentModuleStore m_CurrentModuleStore;
        private readonly IBattleArgsStore    m_BattleArgsStore;
        private readonly IBattleMusicStore   m_BattleMusicStore;
        private readonly IPausedMusicStore   m_PausedMusicStore;
        private readonly IMusicPlayer        m_MusicPlayer;

        // Where a battle goes back to: the module it replaced.
        private byte m_ReturnModuleId;

        public DefaultModuleRouter(ICurrentModuleStore currentModuleStore,
                                   IBattleArgsStore    battleArgsStore,
                                   IBattleMusicStore   battleMusicStore,
                                   IPausedMusicStore   pausedMusicStore,
                                   IMusicPlayer        musicPlayer)
        {
            m_CurrentModuleStore = currentModuleStore;
            m_BattleArgsStore    = battleArgsStore;
            m_BattleMusicStore   = battleMusicStore;
            m_PausedMusicStore   = pausedMusicStore;
            m_MusicPlayer        = musicPlayer;
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

            switch ((moduleId, outcome))
            {
                case (FieldConstants.MODULE_ID, FieldOutcomes.OPEN_MENU):
                    change = ModuleChange.Over(MenuConstants.MODULE_ID);
                    break;
                case (FieldConstants.MODULE_ID, FieldOutcomes.START_BATTLE):
                    m_ReturnModuleId = moduleId;
                    StartBattleMusic();
                    change = ModuleChange.Replace(BattleConstants.MODULE_ID);
                    break;
                case (MenuConstants.MODULE_ID, MenuOutcomes.CLOSED):
                    change = ModuleChange.Close();
                    break;
                case (MenuConstants.MODULE_ID, MenuOutcomes.PLAYTHROUGH):
                    change = ModuleChange.Clear(m_CurrentModuleStore.ModuleId);
                    break;
                case (BattleConstants.MODULE_ID, BattleOutcomes.FINISHED):
                    change = ModuleChange.Replace(m_ReturnModuleId, ModuleEntries.RETURN);
                    break;
                default:
                    throw new InvalidOperationException($"{nameof(DefaultModuleRouter)}::{nameof(Route)} No route from module [{moduleId}] for outcome [{outcome}]");
            }

            return change;
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
    }
}