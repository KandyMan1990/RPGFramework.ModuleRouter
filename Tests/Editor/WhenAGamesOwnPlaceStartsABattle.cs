using NUnit.Framework;
using RPGFramework.Audio.Music;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Store;

namespace RPGFramework.ModuleRouter.Tests
{
    /// <summary>A place of the game's own, its town, starts a battle through the default's one way in.</summary>
    internal sealed class WhenAGamesOwnPlaceStartsABattle
    {
        private TestRouting   m_Routing;
        private IModuleRouter m_Router;
        private ModuleChange  m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Router  = new GameRouter((IDefaultModuleRouter)m_Routing.Router());

            ((ICurrentModuleStore)m_Routing.CurrentModule).SetModuleId(GameRouter.TOWN);

            m_Change = m_Router.Route(GameRouter.TOWN, GameRouter.TOWN_FIGHT);
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
        public void ThenTheMusicIsHandedOverAsAFieldsIs()
        {
            Assert.AreEqual($"pause, play {TestRouting.BATTLE_MUSIC}", m_Routing.Music.Log);
            Assert.IsTrue(((IPausedMusicStore)m_Routing.PausedMusic).TryTake(out MusicSnapshot _));
        }

        [Test]
        public void ThenTheBattleGoesBackToTheTown()
        {
            ((IBattleCompleteStateStore)m_Routing.BattleCompleteState).Set(BattleCompleteState.VICTORY);

            ModuleChange back = m_Router.Route(BattleConstants.MODULE_ID, BattleOutcomes.FINISHED);

            Assert.AreEqual(ModuleChangeKind.Replace, back.Kind);
            Assert.AreEqual(GameRouter.TOWN, back.ModuleId);
            Assert.AreEqual(ModuleEntries.RETURN, back.Entry);
        }
    }
}
