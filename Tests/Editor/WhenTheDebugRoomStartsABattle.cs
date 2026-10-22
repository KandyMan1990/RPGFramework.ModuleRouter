#if ENABLE_RPG_FRAMEWORK_DEBUG
using NUnit.Framework;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core.SharedTypes;
using RPGFramework.DebugRoom.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenTheDebugRoomStartsABattle
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Start;
        private ModuleChange m_End;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();

            IModuleRouter router = m_Routing.Router();

            m_Start = router.Route(DebugRoomConstants.MODULE_ID, DebugRoomOutcomes.BATTLE);

            ((IBattleCompleteStateStore)m_Routing.BattleCompleteState).Set(BattleCompleteState.VICTORY);

            m_End = router.Route(BattleConstants.MODULE_ID, BattleOutcomes.FINISHED);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenTheBattleReplacesTheRoom()
        {
            Assert.AreEqual(ModuleChangeKind.Replace, m_Start.Kind);
            Assert.AreEqual(BattleConstants.MODULE_ID, m_Start.ModuleId);
        }

        [Test]
        public void ThenTheRoomComesBackAfterIt()
        {
            Assert.AreEqual(ModuleChangeKind.Replace, m_End.Kind);
            Assert.AreEqual(DebugRoomConstants.MODULE_ID, m_End.ModuleId);
            Assert.AreEqual(ModuleEntries.RETURN, m_End.Entry);
        }
    }
}
#endif
