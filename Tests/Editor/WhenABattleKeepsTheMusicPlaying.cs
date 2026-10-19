using NUnit.Framework;
using RPGFramework.Audio.Music;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core.Store;
using RPGFramework.Field.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenABattleKeepsTheMusicPlaying
    {
        private TestRouting m_Routing;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();

            ((IBattleArgsStore)m_Routing.BattleArgs).Set(new BattleArgs(0, 0, BattleFlags.USE_CURRENT_MUSIC, 0, 0));

            m_Routing.Router().Route(FieldConstants.MODULE_ID, FieldOutcomes.START_BATTLE);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenTheMusicIsLeftAlone()
        {
            Assert.AreEqual(string.Empty, m_Routing.Music.Log);
        }

        [Test]
        public void ThenNothingIsKeptToResume()
        {
            Assert.IsFalse(((IPausedMusicStore)m_Routing.PausedMusic).TryTake(out MusicSnapshot _));
        }
    }
}
