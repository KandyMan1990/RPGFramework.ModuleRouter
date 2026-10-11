# RPGFramework.ModuleRouter

The default router between the RPG Framework's modules. A module finishing tells Core what happened; Core asks the
game's `IModuleRouter` what follows, and carries it out. This package's `DefaultModuleRouter` holds the framework's own
routes, and a game derives from it for its own.

Requires Unity 6000.6 or newer, RPGFramework.Core and the Core, Field, Menu and Battle shared types — and none of the
modules themselves, so they stay isolated: the router sits above them, as the game's installer does.

---

## The routes

| From | What happened | What follows |
| --- | --- | --- |
| Field | `FieldOutcomes.OPEN_MENU` | the menu opens over the field, which stays loaded and frozen |
| Menu | `MenuOutcomes.CLOSED` | the menu closes, and whatever it opened over resumes |
| Field | `FieldOutcomes.START_BATTLE` | the battle replaces the field |
| Battle | `BattleOutcomes.FINISHED` | the field it replaced comes back, carrying on from where it was |
| Menu | `MenuOutcomes.PLAYTHROUGH` | everything is cleared, and the playthrough's module — `ICurrentModuleStore` — is entered afresh |

Any other outcome throws, naming the module and the outcome, so a route nobody wrote is found on its first use.

## Using it

Bind a router in your global installer. A game with modules of its own — a splash screen, say — derives from
`DefaultModuleRouter`, routes its own, and passes the rest to the base:

```csharp
container.BindSingleton<IModuleRouter, GameModuleRouter>();
```

```csharp
public sealed class GameModuleRouter : DefaultModuleRouter
{
    public GameModuleRouter(ICurrentModuleStore currentModuleStore) : base(currentModuleStore)
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
`ModuleEntries.RETURN`.

## Not in this version

- **Game over**, a battle from the menu, the field's music kept across a battle, and the naming and tutorial screens
  after one are planned routes, not yet built.
