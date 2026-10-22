#if ENABLE_RPG_FRAMEWORK_DEBUG
using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.DebugRoom.SharedTypes.Constants;
using RPGFramework.Field.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenTheDebugRoomJumpsToAField
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Change  = m_Routing.Router().Route(DebugRoomConstants.MODULE_ID, DebugRoomOutcomes.FIELD);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenItClearsToTheFieldEnteredAfresh()
        {
            Assert.AreEqual(ModuleChangeKind.Clear, m_Change.Kind);
            Assert.AreEqual(FieldConstants.MODULE_ID, m_Change.ModuleId);
            Assert.AreEqual(ModuleEntries.NEW, m_Change.Entry);
        }

        [Test]
        public void ThenTheMusicFadesAsAPlaythroughBegins()
        {
            Assert.AreEqual("stop 2", m_Routing.Music.Log);
        }
    }
}
#endif
