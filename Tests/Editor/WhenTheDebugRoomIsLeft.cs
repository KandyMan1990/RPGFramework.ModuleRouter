#if ENABLE_RPG_FRAMEWORK_DEBUG
using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.DebugRoom.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes;
using RPGFramework.Menu.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes.Stores;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenTheDebugRoomIsLeft
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();

            ((IMenuArgsStore)m_Routing.MenuArgs).Set(new MenuArgs((byte)MenuType.Party));

            m_Change = m_Routing.Router().Route(DebugRoomConstants.MODULE_ID, DebugRoomOutcomes.CLOSED);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenTheMenuReplacesTheRoom()
        {
            Assert.AreEqual(ModuleChangeKind.Replace, m_Change.Kind);
            Assert.AreEqual(MenuConstants.MODULE_ID, m_Change.ModuleId);
            Assert.AreEqual(ModuleEntries.NEW, m_Change.Entry);
        }

        [Test]
        public void ThenTheMenuOpensAtTheTitle()
        {
            Assert.AreEqual((byte)MenuType.Begin, ((IMenuArgsStore)m_Routing.MenuArgs).Args.MenuId);
        }
    }
}
#endif
