using RPGFramework.Core.SharedTypes;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    /// <summary>
    /// A game's router: its own splash screen, a place of its own that starts battles, and everything else the
    /// default's.
    /// </summary>
    internal sealed class GameRouter : IModuleRouter
    {
        internal const byte SPLASH     = 40;
        internal const byte TOWN       = 41;
        internal const byte TOWN_FIGHT = 1;

        private readonly IDefaultModuleRouter m_Default;

        internal GameRouter(IDefaultModuleRouter defaultRouter)
        {
            m_Default = defaultRouter;
        }

        ModuleChange IModuleRouter.Route(byte moduleId, byte outcome)
        {
            if (moduleId == SPLASH)
            {
                return ModuleChange.Clear(MenuConstants.MODULE_ID);
            }

            if (moduleId == TOWN && outcome == TOWN_FIGHT)
            {
                return m_Default.StartBattle();
            }

            return m_Default.Route(moduleId, outcome);
        }
    }
}
