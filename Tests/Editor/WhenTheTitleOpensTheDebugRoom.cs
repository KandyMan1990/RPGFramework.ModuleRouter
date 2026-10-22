#if ENABLE_RPG_FRAMEWORK_DEBUG
using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.DebugRoom.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenTheTitleOpensTheDebugRoom
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Change  = m_Routing.Router().Route(MenuConstants.MODULE_ID, DebugMenuOutcomes.DEBUG_ROOM);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenTheRoomReplacesTheMenuEnteredAfresh()
        {
            Assert.AreEqual(ModuleChangeKind.Replace, m_Change.Kind);
            Assert.AreEqual(DebugRoomConstants.MODULE_ID, m_Change.ModuleId);
            Assert.AreEqual(ModuleEntries.NEW, m_Change.Entry);
        }

        [Test]
        public void ThenTheMusicPlaysOn()
        {
            Assert.AreEqual(string.Empty, m_Routing.Music.Log);
        }
    }
}
#endif
