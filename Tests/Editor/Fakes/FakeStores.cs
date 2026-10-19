using RPGFramework.Audio.Music;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core.Store;
using RPGFramework.Field.SharedTypes;
using RPGFramework.Field.SharedTypes.Stores;
using RPGFramework.Menu.SharedTypes;
using RPGFramework.Menu.SharedTypes.Stores;

namespace RPGFramework.ModuleRouter.Tests.Fakes
{
    internal sealed class FakeCurrentModuleStore : ICurrentModuleStore
    {
        private byte m_ModuleId;

        byte ICurrentModuleStore.ModuleId => m_ModuleId;

        void ICurrentModuleStore.SetModuleId(byte moduleId) => m_ModuleId = moduleId;
    }

    internal sealed class FakeBattleArgsStore : IBattleArgsStore
    {
        private BattleArgs m_Args;

        BattleArgs IBattleArgsStore.Args => m_Args;

        void IBattleArgsStore.Set(BattleArgs args) => m_Args = args;
    }

    internal sealed class FakeBattleModeStore : IBattleModeStore
    {
        private BattleFlags m_Mode;

        BattleFlags IBattleModeStore.Mode => m_Mode;

        void IBattleModeStore.SetMode(BattleFlags mode) => m_Mode = mode;
    }

    internal sealed class FakeBattleCompleteStateStore : IBattleCompleteStateStore
    {
        private BattleCompleteState m_State;

        BattleCompleteState IBattleCompleteStateStore.State => m_State;

        void IBattleCompleteStateStore.Set(BattleCompleteState state) => m_State = state;
    }

    internal sealed class FakeBattleMusicStore : IBattleMusicStore
    {
        private ulong m_NameHash;

        ulong IBattleMusicStore.BattleMusic => m_NameHash;

        void IBattleMusicStore.SetBattleMusic(ulong nameHash) => m_NameHash = nameHash;
    }

    /// <summary>Holds whether a snapshot was kept, which is all the router's part in it.</summary>
    internal sealed class FakePausedMusicStore : IPausedMusicStore
    {
        private bool m_Holds;

        void IPausedMusicStore.Set(MusicSnapshot snapshot) => m_Holds = true;

        bool IPausedMusicStore.TryTake(out MusicSnapshot snapshot)
        {
            bool held = m_Holds;

            snapshot = default;
            m_Holds  = false;

            return held;
        }
    }

    internal sealed class FakeFieldArgsStore : IFieldArgsStore
    {
        private FieldArgs m_Args;

        FieldArgs IFieldArgsStore.Args => m_Args;

        void IFieldArgsStore.Set(FieldArgs args) => m_Args = args;
    }

    internal sealed class FakeGameOverFieldStore : IGameOverFieldStore
    {
        private readonly FieldArgs m_Args;

        internal FakeGameOverFieldStore(FieldArgs args)
        {
            m_Args = args;
        }

        FieldArgs IGameOverFieldStore.Args => m_Args;
    }

    internal sealed class FakeMenuArgsStore : IMenuArgsStore
    {
        private MenuArgs m_Args;

        MenuArgs IMenuArgsStore.Args => m_Args;

        void IMenuArgsStore.Set(MenuArgs args) => m_Args = args;
    }
}
