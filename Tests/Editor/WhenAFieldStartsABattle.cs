using NUnit.Framework;
using RPGFramework.Audio.Music;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Store;
using RPGFramework.Field.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenAFieldStartsABattle
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Change  = m_Routing.Router().Route(FieldConstants.MODULE_ID, FieldOutcomes.START_BATTLE);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenTheBattleReplacesIt()
        {
            Assert.AreEqual(ModuleChangeKind.Replace, m_Change.Kind);
            Assert.AreEqual(BattleConstants.MODULE_ID, m_Change.ModuleId);
        }

        [Test]
        public void ThenTheRoutesCoverIsTaken()
        {
            Assert.AreEqual(m_Routing.BattleCover.NameHash, m_Change.Cover);
        }

        [Test]
        public void ThenTheMusicIsPausedBeforeTheBattleMusicPlays()
        {
            Assert.AreEqual($"pause, play {TestRouting.BATTLE_MUSIC}", m_Routing.Music.Log);
        }

        [Test]
        public void ThenWhatWasPausedIsKeptForTheReturn()
        {
            Assert.IsTrue(((IPausedMusicStore)m_Routing.PausedMusic).TryTake(out MusicSnapshot _));
        }
    }
}
