using NUnit.Framework;
using RPGFramework.Core.Store;
using RPGFramework.Field.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenABattleHasNoMusic
    {
        [Test]
        public void ThenTheMusicIsOnlyPaused()
        {
            TestRouting routing = new TestRouting();

            ((IBattleMusicStore)routing.BattleMusic).SetBattleMusic(0);

            routing.Router().Route(FieldConstants.MODULE_ID, FieldOutcomes.START_BATTLE);

            string music = routing.Music.Log;

            routing.Destroy();

            Assert.AreEqual("pause", music);
        }
    }
}
