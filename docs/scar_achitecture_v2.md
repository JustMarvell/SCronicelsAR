# SCAR — Architecture Reference (v0.2)

**Project codename:** SCAR (Sangihe Chronicles / AR — placeholder, adjust as needed)
**Root namespace:** `Scar`
**Engine:** Unity 6000.3, New Input System, AR Foundation (via AR Mobile Template)

> Supersedes `scar_achitecture_v1.md`. That version was the pre-implementation design doc; this version reflects what's actually built through Phase 7 of the MVP roadmap, plus what's still open. Filename typo (`achitecture`) fixed here — delete the old file once you're happy with this one, or keep both for history.

This doc captures the current implementation: folder/namespace structure, the weapon-conditioned AR combat design, the save/persistence model, and MVP scope. Treat it as living — update it as design decisions change. Detailed narrative/level content stays in the repo README, not here.

---

## 1. Folder / namespace structure (as implemented)

```
Assets/_Project/
  Scenes/
    Bootstrap.unity                <- persistent, never unloaded
    Levels/                        <- empty, awaiting real level content
    AR/
      AR_Session.unity             <- single reusable AR scene
    Testing/
      TestExplore.unity            <- throwaway test Explore scene
  Scripts/
    Core/                          -> namespace Scar.Core
      GameModeController.cs        [x]
      SceneLoader.cs                [x]
      EventBus.cs                   [x]
      GameContext.cs                [x]
      (ServiceLocator.cs — not implemented, not currently needed)
    Exploration/                   -> namespace Scar.Exploration
      PlayerController.cs           [x]
      ThirdPersonCamera.cs          [x]
      EncounterTrigger.cs           [x]
      (POIInteractable.cs, NPCTrigger.cs — not implemented)
    AR/                            -> namespace Scar.AR
      ArSessionController.cs        [x] owns AR content-root switching on mode change
      Combat/                      -> namespace Scar.AR.Combat
        IWeaponController.cs        [x]
        MeleeWeaponController.cs    [x]
        RangedWeaponController.cs   [x]
        EnemyCombatant.cs           [x]
        CombatSessionController.cs  [x]
        CombatRewardHandler.cs      [x]
      Dialogue/                    -> namespace Scar.AR.Dialogue (not started)
      ItemInteraction/             -> namespace Scar.AR.ItemInteraction (not started)
    Inventory/                     -> namespace Scar.Inventory
      InventoryManager.cs           [x]
      EquipmentManager.cs           [x]
      ItemPickup.cs                 [x]
      (EquipmentSlot.cs — not implemented; single equipped-weapon field only)
    SaveSystem/                    -> namespace Scar.SaveSystem
      SaveManager.cs                 [x]
      SaveData.cs                    [x]
      CheckpointManager.cs           [x]
      ISaveable.cs                   [x]
      PersistentFlags.cs             [x] generic ISaveable for pickup/objective flags
    Narrative/                     -> namespace Scar.Narrative (not started)
    Data/                          -> namespace Scar.Data (ScriptableObjects)
      WeaponDefinition.cs            [x]
      ItemDefinition.cs              [x]
      (EnemyDefinition.cs, LevelDefinition.cs — not implemented; enemy/level data still hardcoded/scene-based)
    UI/                            -> namespace Scar.UI (not started)
    Utilities/                     -> namespace Scar.Utilities (not started)
    Testing/                       -> namespace Scar.Testing
      TestBootstraper.cs             [x] temp scene-name injector for Play Mode testing
```

**Conventions (unchanged, still followed):**
- One namespace per top-level folder, sub-namespaces mirror subfolders.
- `Scar.Core` has no dependency on any other namespace — everything else can depend on Core, never the reverse.
- Cross-system communication goes through `EventBus` or interfaces (`ISaveable`, `IWeaponController`), not direct references between e.g. `Scar.Exploration` and `Scar.AR.Combat`.

**Dependency graph as implemented:**
```
Scar.Core           <- no dependencies
Scar.SaveSystem     <- Core
Scar.Data           <- no dependencies (plain ScriptableObjects)
Scar.Inventory      <- Core, SaveSystem, Data
Scar.Exploration    <- Core
Scar.AR             <- Core
Scar.AR.Combat      <- Core, Data, Inventory (reads EquipmentManager)
```
No violations of the "Core has zero outbound deps" rule found in current code.

---

## 2. Event catalogue

All cross-system signals go through `EventBus.Publish<T>()` / `Subscribe<T>()`. Current event types:

| Event | Published by | Subscribed by | Payload |
|---|---|---|---|
| `GameModeChangedEvent` | `GameModeController` | `ArSessionController` | `NewMode`, `Context` (GameContext) |
| `EnemyDefeatedEvent` | `EnemyCombatant` | `CombatRewardHandler` | `Enemy` |
| `RewardGrantedEvent` | `CombatRewardHandler` | *(none yet — hook for Inventory reward logic)* | `EnemyId` |
| `ItemCollectedEvent` | `ItemPickup` | `InventoryManager` | `ItemId` |
| `WeaponEquippedEvent` | `EquipmentManager` | *(none yet — hook for UI)* | `Weapon` |

---

## 3. Weapon-conditioned AR combat (as implemented)

Combat behavior is chosen at runtime based on the player's equipped weapon, via `CombatSessionController` reading `EquipmentManager.Instance.EquippedWeapon` and picking `MeleeWeaponController` or `RangedWeaponController` — matches the originally designed strategy pattern.

**`IWeaponController` (Scar.AR.Combat)** — implemented as designed:
```csharp
public interface IWeaponController
{
    void EnterCombat(EnemyCombatant target, WeaponDefinition weapon);
    void Tick();
    void ExitCombat();
}
```

**Melee** — `MeleeWeaponController`: tap input raycasts against `m_EnemyLayer`; attack only lands if the target is within `weapon.Range` and off cooldown. No "move closer" UI cue yet — TODO left in code.

**Ranged** — `RangedWeaponController`: press starts an aim state, release raycasts from the release-time screen position. **Assisted aim resolved as the implementation choice** (not free-aim) — this was an open design question in v0.1, now decided but flagged as revisable pending device feel-testing.

**`CombatSessionController`** — reads `EquipmentManager.Instance.EquippedWeapon` on `OnEnable`, activates the matching controller, calls `EnterCombat`. Enemy target is currently a hardcoded `m_TestEnemy` reference — **not yet resolved from `GameContext.EnemyId`**, since no enemy id → scene-instance resolver exists (would need an `EnemyDefinition` SO + spawn/lookup system, deferred).

---

## 4. Save / persistence model (new since v0.1 — not covered in original doc)

- `SaveManager` (Core-tier singleton) holds a list of registered `ISaveable`s, serializes each via `JsonUtility` to a `SaveData` wrapper, writes JSON to `Application.persistentDataPath/save.json`.
- `ISaveable.CaptureState()` returns a plain object; `RestoreState(object state)` expects a JSON **string** (double-serialization — each saveable's state is itself JSON-encoded within the outer `SaveData.Values` list). This is a deliberate `JsonUtility` workaround, not an oversight.
- `CheckpointManager` is a thin wrapper: `SaveCheckpoint(chapterId)` calls `SaveManager.Save(chapterId, activeSceneName)`; `TryLoadCheckpoint` calls `SaveManager.Load()`.
- **`PersistentFlags`** is the generalized answer to "save objectives/pickups/chapter-progress-adjacent state": a single `ISaveable` storing a `HashSet<string>` of arbitrary flag IDs. `ItemPickup` sets a flag on collection and checks it on scene load (so already-collected pickups deactivate themselves). No dedicated objective system exists yet, but anything needing "done / not done" persistence can reuse this without a new `ISaveable` registration.
- **Known gap:** save data currently captures scene name + chapter id + registered `ISaveable` state, but **not** in-scene sub-area position/progress within a chapter. This ties directly into the still-open "sub-area vs. separate scene per chapter" question below.
- **Execution order caveat:** `PersistentFlags`, `InventoryManager`, and `EquipmentManager` all call `SaveManager.Instance.Register()` in their own `Awake()`. If `SaveManager`'s `Awake()` doesn't run first, this null-refs. Currently unenforced — relies on Bootstrap GameObject ordering or Unity's default execution order. **Flag for a defensive fix** (deferred/queued registration) if this becomes a recurring bug.

---

## 5. Inventory / Equipment (new since v0.1)

- `InventoryManager`: flat list of collected `ItemId` strings (duplicates allowed — no stacking logic beyond count-by-presence). Subscribes to `ItemCollectedEvent`.
- `EquipmentManager`: single `EquippedWeapon` field (no multi-slot equipment yet — armor system not started). `EquipWeapon()` is called externally; currently only wired to a debug trigger, no real UI/flow decision made yet (see open questions).
- `ItemDefinition` (Scar.Data): links a world pickup to an `ItemId` and, optionally, an `EquippableWeapon` (`WeaponDefinition` reference) — this is how a picked-up sword item maps to combat weapon behavior.

---

## 6. MVP scope — vertical slice status

| Phase | Deliverable | Status |
|---|---|---|
| 0 | Project setup: folder structure, namespaces, git, empty Bootstrap scene with placeholder Core managers | Done |
| 1 | `GameModeController` + `SceneLoader`: additive load/unload between a dummy 3D scene and the AR scene, passing a `GameContext` payload | Done |
| 2 | Minimal 3D Explore mode: third-person controller + camera, one small test area, one `EncounterTrigger` | Done |
| 3 | AR session scene: AR Foundation plane/world tracking working, combat sub-mode loads on encounter | Scene/controller wiring done; **tracking behavior itself unverified — Play Mode can't test it, needs device build** |
| 4 | One melee weapon (tap-to-attack) and one ranged weapon (press-hold-release) both working against one test enemy | Done |
| 5 | Reward on combat win → return to 3D mode via `GameModeController` | Done |
| 6 | `SaveManager` + one chapter checkpoint: save on chapter trigger, load on game start | Done — extended beyond original scope to also persist pickups/flags via `PersistentFlags` |
| 7 | Minimal `InventoryManager`/`EquipmentManager`: equip a weapon, `CombatSessionController` correctly picks the matching `IWeaponController` | Done — equip flow itself is debug-only, not a designed player-facing interaction yet |

Everything past this (NPC dialogue in AR, item interaction scenes, full armor/equipment stats, multiple levels, real folklore content, enemy id → world instance resolution, objective system, equip UI) builds on top once device testing confirms the AR side of the loop holds up, not just Play Mode logic.

---

## 7. Not yet decided (flag for later discussion)

- Whether ranged assisted-aim (current implementation) actually feels good on-device, vs. switching to free-aim
- NPC interaction and quick-event AR scene design (unchanged from v0.1, still not started)
- Armor/equipment stat model (what stats exist, how they affect combat) — unchanged from v0.1
- **Chapter checkpoints and sub-area state** — current save format is scene-name-granularity only; doesn't yet capture position/progress within a chapter scene. Needs a decision before chapters get more complex than one test scene.
- Objective system design — flags exist (`PersistentFlags`) but no `ObjectiveManager`, no UI, no defined objective data structure
- Pickup → equip flow (auto-equip on pickup vs. explicit UI action)
- Enemy id → world instance resolution for `CombatSessionController` (currently hardcoded test enemy reference)
- `SaveManager`/`ISaveable` registration execution-order robustness