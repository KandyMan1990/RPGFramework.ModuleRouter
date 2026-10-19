using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Store;
using RPGFramework.Core.Transitions;
using RPGFramework.Field.SharedTypes;
using RPGFramework.Field.SharedTypes.Constants;
using RPGFramework.Field.SharedTypes.Stores;
using RPGFramework.ModuleRouter.Tests.Fakes;
using UnityEditor;
using UnityEngine;

namespace RPGFramework.ModuleRouter.Tests
{
    /// <summary>
    /// The default router over fakes of every store it reads and the music it plays, with a config authored as the
    /// inspector authors it: the battle route covered by a transition of its own, a defeat and a game over fading the
    /// music with their covers, and a playthrough over two seconds.
    /// </summary>
    internal sealed class TestRouting
    {
        internal const byte  CURRENT_MODULE      = 9;
        internal const float PLAYTHROUGH_SECONDS = 2f;
        internal const ulong BATTLE_MUSIC        = 77;

        internal readonly FakeCurrentModuleStore       CurrentModule;
        internal readonly FakeBattleArgsStore          BattleArgs;
        internal readonly FakeBattleModeStore          BattleMode;
        internal readonly FakeBattleCompleteStateStore BattleCompleteState;
        internal readonly FakeBattleMusicStore         BattleMusic;
        internal readonly FakePausedMusicStore         PausedMusic;
        internal readonly FakeFieldArgsStore           FieldArgs;
        internal readonly FakeGameOverFieldStore       GameOverField;
        internal readonly FakeMenuArgsStore            MenuArgs;
        internal readonly FakeMusicPlayer              Music;
        internal readonly FakeTransitionService        Transitions;
        internal readonly ModuleRouterConfig           Config;
        internal readonly ColourTransition             BattleCover;

        internal TestRouting()
        {
            CurrentModule       = new FakeCurrentModuleStore();
            BattleArgs          = new FakeBattleArgsStore();
            BattleMode          = new FakeBattleModeStore();
            BattleCompleteState = new FakeBattleCompleteStateStore();
            BattleMusic         = new FakeBattleMusicStore();
            PausedMusic         = new FakePausedMusicStore();
            FieldArgs           = new FakeFieldArgsStore();
            GameOverField       = new FakeGameOverFieldStore(new FieldArgs(55, 3));
            MenuArgs            = new FakeMenuArgsStore();
            Music               = new FakeMusicPlayer();
            Transitions         = new FakeTransitionService();
            BattleCover         = ScriptableObject.CreateInstance<ColourTransition>();
            Config              = ScriptableObject.CreateInstance<ModuleRouterConfig>();

            BattleCover.name = "RouteBattleCover";

            ((ICurrentModuleStore)CurrentModule).SetModuleId(CURRENT_MODULE);
            ((IBattleMusicStore)BattleMusic).SetBattleMusic(BATTLE_MUSIC);
            ((IBattleArgsStore)BattleArgs).Set(new BattleArgs(0, 0, BattleFlags.DEFAULT, 0, 0));

            SerializedObject config = new SerializedObject(Config);

            config.FindProperty("m_BattleStarting.m_Cover").objectReferenceValue = BattleCover;
            config.FindProperty("m_Defeat.m_MusicFade").enumValueIndex            = (int)MusicFade.WithCover;
            config.FindProperty("m_GameOver.m_MusicFade").enumValueIndex          = (int)MusicFade.WithCover;
            config.FindProperty("m_Playthrough.m_MusicFade").enumValueIndex       = (int)MusicFade.OverSeconds;
            config.FindProperty("m_Playthrough.m_MusicFadeSeconds").floatValue    = PLAYTHROUGH_SECONDS;
            config.ApplyModifiedPropertiesWithoutUndo();
        }

        internal IModuleRouter Router()
        {
            IModuleRouter router = new DefaultModuleRouter(CurrentModule, BattleArgs, BattleMode, BattleCompleteState, BattleMusic, PausedMusic, FieldArgs, GameOverField, MenuArgs,
                                                           Music, Config, Transitions);

            return router;
        }

        /// <summary>Starts a battle from the field, and readies it to end as <paramref name="state" />.</summary>
        internal void Fight(IModuleRouter router, BattleCompleteState state, BattleFlags mode)
        {
            router.Route(FieldConstants.MODULE_ID, FieldOutcomes.START_BATTLE);

            ((IBattleCompleteStateStore)BattleCompleteState).Set(state);
            ((IBattleModeStore)BattleMode).SetMode(mode);
            ((IFieldArgsStore)FieldArgs).Set(new FieldArgs(1, 1));

            Music.ClearLog();
        }

        internal void Destroy()
        {
            Object.DestroyImmediate(Config);
            Object.DestroyImmediate(BattleCover);
        }
    }
}
