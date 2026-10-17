using System;
using RPGFramework.Core.SharedTypes;
using RPGFramework.Core.Transitions;
using UnityEngine;

namespace RPGFramework.ModuleRouter
{
    /// <summary>How each of the default router's routes covers and reveals the screen, and fades the music.</summary>
    public interface IModuleRouterConfig
    {
        /// <summary>The menu opening over the field.</summary>
        RouteTransition MenuOpening { get; }

        /// <summary>The menu closing back to the field.</summary>
        RouteTransition MenuClosing { get; }

        /// <summary>A battle starting; a battle that names its own cover uses that instead.</summary>
        RouteTransition BattleStarting { get; }

        /// <summary>A battle ending, back to where it started.</summary>
        RouteTransition BattleEnding { get; }

        /// <summary>A lost battle going to the game-over field.</summary>
        RouteTransition Defeat { get; }

        /// <summary>A game over, back to the title.</summary>
        RouteTransition GameOver { get; }

        /// <summary>A playthrough begun or loaded from the menu.</summary>
        RouteTransition Playthrough { get; }
    }

    /// <summary>Whether a route fades the music out as its cover starts, and over how long.</summary>
    public enum MusicFade : byte
    {
        /// <summary>The music plays on, for the next module to change or keep.</summary>
        None,

        /// <summary>Faded out over the route's cover, in step with the screen.</summary>
        WithCover,

        /// <summary>Faded out over seconds of its own, which may outlast the reveal.</summary>
        OverSeconds
    }

    /// <summary>A route's cover and reveal, either left empty for the game's defaults, and its music fade.</summary>
    [Serializable]
    public struct RouteTransition
    {
        [SerializeField]
        private TransitionAsset m_Cover;

        [SerializeField]
        private TransitionAsset m_Reveal;

        [SerializeField]
        private MusicFade m_MusicFade;

        [SerializeField]
        [Min(0f)]
        [Tooltip("Seconds the music fades over, when it fades over seconds of its own")]
        private float m_MusicFadeSeconds;

        internal MusicFade MusicFade        => m_MusicFade;
        internal float     MusicFadeSeconds => m_MusicFadeSeconds;

        internal ModuleChange Apply(ModuleChange change)
        {
            ulong cover  = m_Cover  != null ? m_Cover.NameHash : 0;
            ulong reveal = m_Reveal != null ? m_Reveal.NameHash : 0;

            ModuleChange applied = change.WithTransitions(cover, reveal);

            return applied;
        }
    }

    [CreateAssetMenu(menuName = "RPG Framework/Module Router Config", fileName = "ModuleRouterConfig")]
    public sealed class ModuleRouterConfig : ScriptableObject, IModuleRouterConfig
    {
        [SerializeField]
        private RouteTransition m_MenuOpening;

        [SerializeField]
        private RouteTransition m_MenuClosing;

        [SerializeField]
        private RouteTransition m_BattleStarting;

        [SerializeField]
        private RouteTransition m_BattleEnding;

        [SerializeField]
        private RouteTransition m_Defeat;

        [SerializeField]
        private RouteTransition m_GameOver;

        [SerializeField]
        private RouteTransition m_Playthrough;

        RouteTransition IModuleRouterConfig.MenuOpening    => m_MenuOpening;
        RouteTransition IModuleRouterConfig.MenuClosing    => m_MenuClosing;
        RouteTransition IModuleRouterConfig.BattleStarting => m_BattleStarting;
        RouteTransition IModuleRouterConfig.BattleEnding   => m_BattleEnding;
        RouteTransition IModuleRouterConfig.Defeat         => m_Defeat;
        RouteTransition IModuleRouterConfig.GameOver       => m_GameOver;
        RouteTransition IModuleRouterConfig.Playthrough    => m_Playthrough;
    }
}