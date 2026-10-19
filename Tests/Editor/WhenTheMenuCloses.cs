using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenTheMenuCloses
    {
        [Test]
        public void ThenItClosesBackToWhatItOpenedOver()
        {
            TestRouting  routing = new TestRouting();
            ModuleChange change  = routing.Router().Route(MenuConstants.MODULE_ID, MenuOutcomes.CLOSED);

            routing.Destroy();

            Assert.AreEqual(ModuleChangeKind.Close, change.Kind);
        }
    }
}
