using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Field.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes;
using RPGFramework.Menu.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes.Stores;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenTheGameIsOver
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();

            ((IMenuArgsStore)m_Routing.MenuArgs).Set(new MenuArgs((byte)MenuType.Party));

            m_Change = m_Routing.Router().Route(FieldConstants.MODULE_ID, FieldOutcomes.GAME_OVER);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenItClearsToTheMenu()
        {
            Assert.AreEqual(ModuleChangeKind.Clear, m_Change.Kind);
            Assert.AreEqual(MenuConstants.MODULE_ID, m_Change.ModuleId);
        }

        [Test]
        public void ThenTheMenuOpensAtTheTitle()
        {
            Assert.AreEqual((byte)MenuType.Begin, ((IMenuArgsStore)m_Routing.MenuArgs).Args.MenuId);
        }

        [Test]
        public void ThenTheMusicFadesWithTheCover()
        {
            Assert.AreEqual("stop 0.75", m_Routing.Music.Log);
        }
    }
}
