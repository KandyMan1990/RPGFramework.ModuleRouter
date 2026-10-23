#if ENABLE_RPG_FRAMEWORK_DEBUG
using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.DebugRoom.SharedTypes.Constants;
using RPGFramework.Field.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenAFieldGoesToTheDebugRoom
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Change  = m_Routing.Router().Route(FieldConstants.MODULE_ID, DebugFieldOutcomes.DEBUG_ROOM);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenItClearsToTheRoomEnteredAfresh()
        {
            Assert.AreEqual(ModuleChangeKind.Clear, m_Change.Kind);
            Assert.AreEqual(DebugRoomConstants.MODULE_ID, m_Change.ModuleId);
            Assert.AreEqual(ModuleEntries.NEW, m_Change.Entry);
        }

        [Test]
        public void ThenTheMusicStops()
        {
            Assert.AreEqual("stop 0.001", m_Routing.Music.Log);
        }
    }
}
#endif
