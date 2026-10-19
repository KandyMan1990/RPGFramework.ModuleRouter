using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Field.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenAGameRoutesItsOwnModule
    {
        private TestRouting   m_Routing;
        private IModuleRouter m_Router;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Router  = new GameRouter(m_Routing);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenItsRouterRoutesIt()
        {
            ModuleChange change = m_Router.Route(GameRouter.SPLASH, 1);

            Assert.AreEqual(ModuleChangeKind.Clear, change.Kind);
            Assert.AreEqual(MenuConstants.MODULE_ID, change.ModuleId);
        }

        [Test]
        public void ThenTheFrameworksAreLeftToTheDefault()
        {
            ModuleChange change = m_Router.Route(FieldConstants.MODULE_ID, FieldOutcomes.OPEN_MENU);

            Assert.AreEqual(ModuleChangeKind.Over, change.Kind);
            Assert.AreEqual(MenuConstants.MODULE_ID, change.ModuleId);
        }
    }
}
