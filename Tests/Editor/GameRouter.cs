using RPGFramework.Core.SharedTypes;
using RPGFramework.Menu.SharedTypes.Constants;

namespace RPGFramework.ModuleRouter.Tests
{
    /// <summary>A game's router: its own splash screen, everything else the default's.</summary>
    internal sealed class GameRouter : DefaultModuleRouter
    {
        internal const byte SPLASH = 40;

        internal GameRouter(TestRouting routing)
            : base(routing.CurrentModule, routing.BattleArgs, routing.BattleMode, routing.BattleCompleteState, routing.BattleMusic, routing.PausedMusic, routing.FieldArgs,
                   routing.GameOverField, routing.MenuArgs, routing.Music, routing.Config, routing.Transitions)
        {
        }

        protected override ModuleChange Route(byte moduleId, byte outcome)
        {
            if (moduleId == SPLASH)
            {
                return ModuleChange.Clear(MenuConstants.MODULE_ID);
            }

            return base.Route(moduleId, outcome);
        }
    }
}
