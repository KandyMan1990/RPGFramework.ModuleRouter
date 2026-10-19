using NUnit.Framework;
using RPGFramework.Audio.Music;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Store;
using RPGFramework.Field.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenABattleIsWon
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();

            IModuleRouter router = m_Routing.Router();

            m_Routing.Fight(router, BattleCompleteState.VICTORY, BattleFlags.DEFAULT);

            m_Change = router.Route(BattleConstants.MODULE_ID, BattleOutcomes.FINISHED);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenItReturnsToTheFieldItReplaced()
        {
            Assert.AreEqual(ModuleChangeKind.Replace, m_Change.Kind);
            Assert.AreEqual(FieldConstants.MODULE_ID, m_Change.ModuleId);
            Assert.AreEqual(ModuleEntries.RETURN, m_Change.Entry);
        }

        [Test]
        public void ThenThePausedMusicIsLeftForTheField()
        {
            Assert.IsTrue(((IPausedMusicStore)m_Routing.PausedMusic).TryTake(out MusicSnapshot _));
        }
    }
}
