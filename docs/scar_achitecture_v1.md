# SCAR — Architecture Reference (v0.1)

**Project codename:** SCAR (Sangihe Chronicles / AR — placeholder, adjust as needed)
**Root namespace:** `Scar`
**Engine:** Unity 6000.3, New Input System, AR Foundation (via AR Mobile Template)

This doc captures the decisions made so far: folder/namespace structure, the weapon-conditioned AR combat design, and the MVP scope. Treat it as living — update it as design decisions change. Detailed narrative/level content stays in the repo README, not here.

---

## 1. Folder / namespace structure

```
Assets/
  _Project/
    Scenes/
      Bootstrap.unity                <- persistent, never unloaded
      Levels/
        Level01_<Name>/
          Chapter01.unity
          Chapter02.unity
          ...
      AR/
        AR_Session.unity             <- single reusable AR scene
    Scripts/
      Core/                          -> namespace Scar.Core
        GameModeController.cs
        SceneLoader.cs
        EventBus.cs
        ServiceLocator.cs
        GameContext.cs               <- data passed between modes on switch
      Exploration/                   -> namespace Scar.Exploration
        PlayerController.cs
        ThirdPersonCamera.cs
        EncounterTrigger.cs
        POIInteractable.cs
        NPCTrigger.cs
      AR/                            -> namespace Scar.AR
        ArSessionController.cs       <- owns the persistent AR session lifecycle
        Combat/                      -> namespace Scar.AR.Combat
          CombatSessionController.cs
          IWeaponController.cs
          MeleeWeaponController.cs
          RangedWeaponController.cs
          EnemyCombatant.cs
          CombatRewardHandler.cs
        Dialogue/                    -> namespace Scar.AR.Dialogue   (tbd, later phase)
        ItemInteraction/             -> namespace Scar.AR.ItemInteraction (tbd, later phase)
      Inventory/                     -> namespace Scar.Inventory
        InventoryManager.cs
        EquipmentManager.cs
        EquipmentSlot.cs
        ItemPickup.cs
      SaveSystem/                    -> namespace Scar.SaveSystem
        SaveManager.cs
        SaveData.cs
        CheckpointManager.cs
        ISaveable.cs
      Narrative/                     -> namespace Scar.Narrative     (tbd, later phase)
        ChapterManager.cs
        StoryFlags.cs
      Data/                          -> namespace Scar.Data           (ScriptableObjects)
        WeaponDefinition.cs
        ItemDefinition.cs
        EnemyDefinition.cs
        LevelDefinition.cs
      UI/                            -> namespace Scar.UI
      Utilities/                     -> namespace Scar.Utilities
```

**Conventions:**
- One namespace per top-level folder, sub-namespaces mirror subfolders (`Scar.AR.Combat`, not a flat `Scar.AR` dumping ground).
- `Scar.Core` has no dependency on any other namespace — everything else can depend on Core, never the reverse. This keeps the mode-switching backbone stable while gameplay systems change underneath it.
- Cross-system communication goes through `EventBus` or interfaces (`ISaveable`, `IInteractable`), not direct references between e.g. `Scar.Exploration` and `Scar.AR.Combat`. This is what makes the AR scenes reusable — the AR side doesn't need to know which level scene called it, just the `GameContext` payload.

---

## 2. Weapon-conditioned AR combat

Combat behavior is chosen at runtime based on the player's equipped weapon, via a strategy pattern rather than an if/else in the combat controller.

**`IWeaponController` (Scar.AR.Combat)**
```csharp
public interface IWeaponController
{
    void EnterCombat(EnemyCombatant target, WeaponDefinition weapon);
    void Tick();
    void ExitCombat();
}
```

**Melee (sword, spear):**
- `MeleeWeaponController` — active only when the player's AR-tracked distance to the enemy anchor is within `weapon.range`.
- Tap-on-enemy input → damage event, subject to weapon's attack cooldown.
- Out-of-range tap → no-op or a "move closer" UI cue.

**Ranged (bow, slingshot, thrown rock):**
- `RangedWeaponController` — press-and-hold starts an aim state (could show a trajectory/reticle), drag while held to adjust aim, release fires.
- Release computes hit/miss against the enemy anchor at release time — decide early whether aiming is purely 2D screen-space (reticle over enemy on screen) or uses device orientation/AR raycast, since that changes the input plumbing.

**`CombatSessionController`** reads the player's currently equipped `WeaponDefinition` from `EquipmentManager` when the AR combat scene loads, and instantiates/enables the matching `IWeaponController`. This is the one place that knows "sword → melee controller, bow → ranged controller" — everything else just talks to the interface.

**Open question to resolve before implementation:** for ranged weapons, do we want full free-aim (player physically points the device/camera) or assisted aim (drag a reticle, engine handles hit detection)? This affects difficulty, accessibility, and how forgiving combat feels — worth deciding with a quick prototype rather than locking it in on paper.

---

## 3. MVP scope — vertical slice

Goal: prove the full mode-switching loop end to end before expanding content or systems.

| Phase | Deliverable |
|---|---|
| 0 | Project setup: folder structure, namespaces, git, empty Bootstrap scene with placeholder Core managers |
| 1 | `GameModeController` + `SceneLoader`: additive load/unload between a dummy 3D scene and the AR scene, passing a `GameContext` payload |
| 2 | Minimal 3D Explore mode: third-person controller + camera, one small test area, one `EncounterTrigger` |
| 3 | AR session scene: AR Foundation plane/world tracking working, combat sub-mode loads on encounter |
| 4 | One melee weapon (tap-to-attack) and one ranged weapon (press-hold-release) both working against one test enemy, to validate both interaction patterns early |
| 5 | Reward on combat win → return to 3D mode via `GameModeController` |
| 6 | `SaveManager` + one chapter checkpoint: save on chapter trigger, load on game start |
| 7 | Minimal `InventoryManager`/`EquipmentManager`: equip a weapon, `CombatSessionController` correctly picks the matching `IWeaponController` |

Everything past this (NPC dialogue in AR, item interaction scenes, full inventory/armor stats, multiple levels, the actual Sangihe folklore content) builds on top once this loop is solid and tested on an actual phone, not just in Play Mode.

---

## 4. Not yet decided (flag for later discussion)
- Free-aim vs assisted-aim for ranged weapons (see above)
- NPC interaction and quick-event AR scene design (marked TBD in original spec)
- Armor/equipment stat model (what stats exist, how they affect combat)
- Save data format (JSON vs binary) and whether saves need encryption
- Whether chapter checkpoints also gate 3D scene loading (i.e. does loading a chapter load a specific sub-area of the level scene, or a whole new scene per chapter)