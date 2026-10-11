using System;
using RPGFramework.Battle.SharedTypes.Constants;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Store;
using RPGFramework.Field.SharedTypes.Constants;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter
{
    /// <summary>
    /// The framework's own routes between its modules: the menu over the field and closing back to it, a battle replacing
    /// the field and returning to it, and a playthrough begun or loaded from the menu. A game derives from it for routes
    /// of its own, overriding <see cref="Route" /> and passing the rest to the base.
    /// </summary>
    public class DefaultModuleRouter : IModuleRouter
    {
        private readonly ICurrentModuleStore m_CurrentModuleStore;

        // Where a battle goes back to: the module it replaced.
        private byte m_ReturnModuleId;

        public DefaultModuleRouter(ICurrentModuleStore currentModuleStore)
        {
            m_CurrentModuleStore = currentModuleStore;
        }

        ModuleChange IModuleRouter.Route(byte moduleId, byte outcome)
        {
            ModuleChange change = Route(moduleId, outcome);

            return change;
        }

        /// <summary>
        /// What follows <paramref name="outcome" /> from the module on top, <paramref name="moduleId" />. Throws for an
        /// outcome with no route.
        /// </summary>
        protected virtual ModuleChange Route(byte moduleId, byte outcome)
        {
            ModuleChange change;

            switch ((moduleId, outcome))
            {
                case (FieldConstants.MODULE_ID, FieldOutcomes.OPEN_MENU):
                    change = ModuleChange.Over(MenuConstants.MODULE_ID);
                    break;
                case (FieldConstants.MODULE_ID, FieldOutcomes.START_BATTLE):
                    m_ReturnModuleId = moduleId;
                    change           = ModuleChange.Replace(BattleConstants.MODULE_ID);
                    break;
                case (MenuConstants.MODULE_ID, MenuOutcomes.CLOSED):
                    change = ModuleChange.Close();
                    break;
                case (MenuConstants.MODULE_ID, MenuOutcomes.PLAYTHROUGH):
                    change = ModuleChange.Clear(m_CurrentModuleStore.ModuleId);
                    break;
                case (BattleConstants.MODULE_ID, BattleOutcomes.FINISHED):
                    change = ModuleChange.Replace(m_ReturnModuleId, ModuleEntries.RETURN);
                    break;
                default:
                    throw new InvalidOperationException($"{nameof(DefaultModuleRouter)}::{nameof(Route)} No route from module [{moduleId}] for outcome [{outcome}]");
            }

            return change;
        }
    }
}