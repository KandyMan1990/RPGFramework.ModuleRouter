using System;
using NUnit.Framework;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenAnOutcomeHasNoRoute
    {
        [Test]
        public void ThenItIsRefused()
        {
            TestRouting routing = new TestRouting();

            Assert.Throws<InvalidOperationException>(() => routing.Router().Route(MenuConstants.MODULE_ID, 200));

            routing.Destroy();
        }
    }
}
