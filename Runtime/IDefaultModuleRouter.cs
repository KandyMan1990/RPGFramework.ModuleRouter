using RPGFramework.Core.SharedTypes;

namespace RPGFramework.ModuleRouter
{
    /// <summary>
    /// The framework's routes, for a game's own router to hand on what it does not route and to start a battle from a
    /// module of its own. A game with no routes of its own binds <see cref="DefaultModuleRouter" /> as its
    /// <see cref="IModuleRouter" />; a game with some binds it as this, and its own router, which takes this, as
    /// <see cref="IModuleRouter" />.
    /// </summary>
    public interface IDefaultModuleRouter : IModuleRouter
    {
        /// <summary>
        /// A battle replacing the module on top, from the <c>BattleArgs</c> the module stored: the music playing paused for
        /// the place to resume and the battle music started, the battle's own cover over the route's, and, as it ends,
        /// back to the place the player is in, Core's <c>ICurrentModuleStore</c>, as a field's battle goes back to it.
        /// </summary>
        ModuleChange StartBattle();
    }
}
