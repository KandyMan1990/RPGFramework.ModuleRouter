using NUnit.Framework;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    internal sealed class WhenAPlaythroughBegins
    {
        private TestRouting  m_Routing;
        private ModuleChange m_Change;

        [SetUp]
        public void SetUp()
        {
            m_Routing = new TestRouting();
            m_Change  = m_Routing.Router().Route(MenuConstants.MODULE_ID, MenuOutcomes.PLAYTHROUGH);
        }

        [TearDown]
        public void TearDown()
        {
            m_Routing.Destroy();
        }

        [Test]
        public void ThenItClearsToTheModuleTheSaveIsInEnteredAfresh()
        {
            Assert.AreEqual(ModuleChangeKind.Clear, m_Change.Kind);
            Assert.AreEqual(TestRouting.CURRENT_MODULE, m_Change.ModuleId);
            Assert.AreEqual(ModuleEntries.NEW, m_Change.Entry);
        }

        [Test]
        public void ThenTheMusicFadesOverItsOwnSeconds()
        {
            Assert.AreEqual("stop 2", m_Routing.Music.Log);
        }
    }
}
