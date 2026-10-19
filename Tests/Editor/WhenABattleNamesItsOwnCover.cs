using NUnit.Framework;
using RPGFramework.Battle.SharedTypes;
using RPGFramework.Battle.SharedTypes.Stores;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Field.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenABattleNamesItsOwnCover
    {
        [Test]
        public void ThenItCoversTheBattleInsteadOfTheRoutes()
        {
            TestRouting routing = new TestRouting();

            ((IBattleArgsStore)routing.BattleArgs).Set(new BattleArgs(0, 0, BattleFlags.DEFAULT, 0, 99));

            ModuleChange change = routing.Router().Route(FieldConstants.MODULE_ID, FieldOutcomes.START_BATTLE);

            routing.Destroy();

            Assert.AreEqual(99UL, change.Cover);
        }
    }
}
