using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Field.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenTheFieldOpensTheMenu
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Change  = m_Routing.Router().Route(FieldConstants.MODULE_ID, FieldOutcomes.OPEN_MENU);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenTheMenuOpensOverIt()
        {
            Assert.AreEqual(ModuleChangeKind.Over, m_Change.Kind);
            Assert.AreEqual(MenuConstants.MODULE_ID, m_Change.ModuleId);
        }

        [Test]
        public void ThenTheMusicIsLeftAlone()
        {
            Assert.AreEqual(string.Empty, m_Routing.Music.Log);
        }
    }
}
