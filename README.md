# RPGFramework.ModuleRouter

The default router between the RPG Framework's modules. A module finishing tells Core what happened; Core asks the
game's `IModuleRouter` what follows, and carries it out. This package's `DefaultModuleRouter` holds the framework's own
routes, and a game derives from it for its own.

Requires Unity 6000.6 or newer, RPGFramework.Core, RPGFramework.Audio and the Core, Field, Menu, Battle and Debug Room
shared types — and none of the modules themselves, so they stay isolated: the router sits above them, as the game's
installer does.

---

## The routes

| From | What happened | What follows |
| --- | --- | --- |
| Field | `FieldOutcomes.OPEN_MENU` | the menu opens over the field, which stays loaded and frozen |
| Menu | `MenuOutcomes.CLOSED` | the menu closes, and whatever it opened over resumes |
| Field | `FieldOutcomes.START_BATTLE` | the battle replaces the field, after the music is handed over (below) |
| Battle | `BattleOutcomes.FINISHED`, won or escaped | the field it replaced comes back, carrying on from where it was |
| Battle | `BattleOutcomes.FINISHED`, lost or out of time | the game-over field — `IGameOverFieldStore`'s — entered afresh, unless the battle mode allows it (below), when the field comes back as after a win |
| Field | `FieldOutcomes.GAME_OVER` | everything is cleared, and the menu is entered at its title, `MenuType.Begin` |
| Menu | `MenuOutcomes.PLAYTHROUGH` | everything is cleared, and the playthrough's module — `ICurrentModuleStore` — is entered afresh |

Any other outcome throws, naming the module and the outcome, so a route nobody wrote is found on its first use.

**The debug room's routes**, compiled only with `ENABLE_RPG_FRAMEWORK_DEBUG` (the Debug Room package), take Core's own
transitions and add nothing to your config:

| From | What happened | What follows |
| --- | --- | --- |
| Menu | `DebugMenuOutcomes.DEBUG_ROOM` | the music stops, and the room replaces the menu |
| Field | `DebugFieldOutcomes.DEBUG_ROOM` | the music stops, and everything is cleared and the room entered, the playthrough carrying on |
| Debug room | `DebugRoomOutcomes.CLOSED` | the menu replaces the room, at its title |
| Debug room | `DebugRoomOutcomes.FIELD` | everything is cleared and the field is entered afresh, as a playthrough begins |
| Debug room | `DebugRoomOutcomes.BATTLE` | the battle replaces the room, as from the field, and comes back to it |
| Debug room | `DebugRoomOutcomes.SCENARIO` | everything is cleared and the module the scenario was saved in entered afresh, as a playthrough begins |

**The music as a battle begins.** While the field is still on screen, the router pauses the music playing and starts
the battle music — `IBattleMusicStore`'s, the track every battle plays until a script sets another, or none if it is
zero. The paused song goes in Core's `IPausedMusicStore`, and the field resumes it as the battle returns to it. A
battle with `BattleFlags.USE_CURRENT_MUSIC` leaves the music playing, and nothing is paused.

**Losing a battle.** Whether a defeat, or a countdown running out, ends the game is the battle mode's to say —
`IBattleModeStore`, a saved setting a script changes — through `BattleFlags.DISABLE_GAME_OVER_ON_DEFEAT` and
`DISABLE_GAME_OVER_ON_TIME_OUT`, read after every battle; one battle's own flags never decide it. How the battle ended is
`IBattleCompleteStateStore`'s, which the field's scripts read too.

**Transitions and the music on the way out are yours to set.** Bind a **Module Router Config** (Create > RPG
Framework > Module Router Config) as `IModuleRouterConfig`. For each route — the menu opening and closing, a battle
starting and ending, a defeat, a game over, a playthrough — it holds the transition that covers the screen and the one
that reveals it, either left empty for your Transition Config's defaults, and whether the music fades out as the cover
starts: not at all, over the cover, or over seconds of its own, which can run on past the reveal. Core carries the
transitions out; the router fades the music, asking Core's `ITransitionService` how long the cover takes. **A battle may name
its own cover** — a field script's `START_BATTLE` names one — and it is used over the route's, which is how a boss
battle gets its own.

## Using it

Bind a router in your global installer. A game with modules of its own — a splash screen, say — derives from
`DefaultModuleRouter`, routes its own, and passes the rest to the base:

```csharp
container.BindSingleton<IModuleRouter, GameModuleRouter>();
```

```csharp
public sealed class GameModuleRouter : DefaultModuleRouter
{
    public GameModuleRouter(ICurrentModuleStore       currentModuleStore,
                            IBattleArgsStore          battleArgsStore,
                            IBattleModeStore          battleModeStore,
                            IBattleCompleteStateStore battleCompleteStateStore,
                            IBattleMusicStore         battleMusicStore,
                            IPausedMusicStore         pausedMusicStore,
                            IFieldArgsStore           fieldArgsStore,
                            IGameOverFieldStore       gameOverFieldStore,
                            IMenuArgsStore            menuArgsStore,
                            IMusicPlayer              musicPlayer,
                            IModuleRouterConfig       config,
                            ITransitionService        transitions)
        : base(currentModuleStore, battleArgsStore, battleModeStore, battleCompleteStateStore, battleMusicStore,
               pausedMusicStore, fieldArgsStore, gameOverFieldStore, menuArgsStore, musicPlayer, config, transitions)
    {
    }

    protected override ModuleChange Route(byte moduleId, byte outcome)
    {
        if (moduleId == SplashScreenConstants.MODULE_ID)
        {
            ModuleChange toTitle = ModuleChange.Clear(MenuConstants.MODULE_ID);

            return toTitle;
        }

        ModuleChange change = base.Route(moduleId, outcome);

        return change;
    }
}
```

A route answers with a `ModuleChange`:

- **`Over`** opens a module over the one on top, which is suspended and stays loaded;
- **`Close`** closes the top, and resumes the one under it;
- **`Replace`** exits the top and enters another in its place;
- **`Clear`** exits them all and enters another alone.

Each takes how the module is entered, `ModuleEntries.NEW` unless it is returning to a module a battle replaced, with
`ModuleEntries.RETURN`. `WithTransitions` names the transitions that cover and reveal it, by the hash of their names in
your transition database (`TransitionAsset.NameHash`).

## Tests

The tests are in `Tests/Editor`, on Unity's Test Framework. They compile only in a project that lists the package as
testable: add it to `testables` in `Packages/manifest.json`,

```json
"testables": [
  "com.rpgframework.modulerouter"
]
```

then open **Window › General › Test Runner** and run the **EditMode** tests.

## Not in this version

- **A battle from the menu**, and the naming and tutorial screens after a battle, are planned routes, not yet built.
- **Trying a battle again, or carrying on from a checkpoint**, after a game over, are planned and not built.
