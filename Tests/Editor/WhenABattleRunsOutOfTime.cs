using NUnit.Framework;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Store;

namespace RPGFramework.ModuleRouter.Tests
{
    /// <summary>A battle carrying the countdown runs out of time, the battle mode letting play carry on or not.</summary>
    internal sealed class WhenABattleRunsOutOfTime
    {
        [TestCase(BattleFlags.DISABLE_GAME_OVER_ON_TIME_OUT)]
        [TestCase(BattleFlags.DEFAULT)]
        public void ThenTheCountdownIsHidden(BattleFlags mode)
        {
            TestRouting routing = RunOut(mode);

            bool shown = ((ICountdownStore)routing.Countdown).IsShown;

            routing.Destroy();

            Assert.IsFalse(shown);
        }

        [TestCase(BattleFlags.DISABLE_GAME_OVER_ON_TIME_OUT)]
        [TestCase(BattleFlags.DEFAULT)]
        public void ThenBattlesStopCarryingIt(BattleFlags mode)
        {
            TestRouting routing = RunOut(mode);

            BattleFlags after = ((IBattleModeStore)routing.BattleMode).Mode;

            routing.Destroy();

            Assert.AreEqual(BattleFlags.DEFAULT, after & BattleFlags.INHERIT_COUNTDOWN_TIMER);
        }

        private static TestRouting RunOut(BattleFlags mode)
        {
            TestRouting   routing = new TestRouting();
            IModuleRouter router  = routing.Router();

            ((ICountdownStore)routing.Countdown).SetShown(true);

            routing.Fight(router, BattleCompleteState.OUT_OF_TIME, mode | BattleFlags.INHERIT_COUNTDOWN_TIMER);
            router.Route(BattleConstants.MODULE_ID, BattleOutcomes.FINISHED);

            return routing;
        }
    }
}
