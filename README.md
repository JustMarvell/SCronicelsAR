# SCAR — Sangihe Chronicles AR (working title)

An interactive, story-driven AR mobile game inspired by folklore and legends of the Sangihe Archipelago. The player alternates between third-person 3D exploration and AR-based combat/interaction as they move through story levels and chapters.

> Design details evolve as development progresses — this README reflects the current state of the design and implementation. See `/docs` for deeper architecture rationale.

---

## Tech stack

| | |
|---|---|
| Engine | Unity 6000.3 |
| Input | Unity's New Input System |
| AR | AR Foundation, built on the Unity AR Mobile Project Template |
| Root namespace | `Scar` |

**Target platforms:** Mobile (AR gameplay). Editor Play Mode is used for iteration on non-AR logic only — AR tracking/performance must be validated on an actual device, since Play Mode does not represent real-world AR conditions.

---

## Game concept

- **Genre:** Story-driven AR RPG, narrative-first.
- **Structure:** The game is split into **levels**, each telling a distinct story drawn from Sangihe folklore. Each level is split into **chapters**, which double as save checkpoints.
- **Two gameplay modes per level:**
  - **3D Explore Mode** — third-person open-world-style exploration (reference: Genshin Impact, Sekiro, Zenless Zone Zero camera/movement feel; reference: Honkai Star Rail's split between exploration and a distinct combat mode). Used for traversal, discovering POIs, finding story items, and NPC interaction triggers.
  - **AR Mode** — used for combat, and eventually narrative/interaction beats (NPC dialogue, item interaction, quick events — design TBD). Entered from Explore Mode (e.g. on enemy encounter) and returns to Explore Mode when finished.

## Core systems

- **Inventory system** — holds items collected during exploration. *(implemented, minimal)*
- **Equipment/armor system** — equippable gear affecting the player character. *(implemented for weapons; armor stats TBD)*
- **Pickup/equip system** — connects world items to the inventory and equipment systems. *(implemented, manual-equip only — see Open design questions)*
- **Save/load system** — checkpoint-based, tied to chapter progression; supports quicksave and manual load. *(implemented)*
- **AR combat** — weapon-conditioned combat behavior: *(implemented)*
  - **Melee weapons** (sword, spear): player must close the AR-tracked distance to within weapon range, then tap the enemy to attack.
  - **Ranged weapons** (bow, slingshot, throwable): press-and-hold to aim, release to fire. Currently **assisted aim** (screen-space raycast on release) — see Open design questions.
  - Combat behavior is selected at runtime based on the player's equipped weapon (strategy pattern), not hardcoded per encounter.

## Architecture overview

- A **persistent Bootstrap scene** holds core managers (`Scar.Core`: `GameModeController`, `SceneLoader`, `EventBus`; `Scar.SaveSystem`: `SaveManager`, `CheckpointManager`, `PersistentFlags`; `Scar.Inventory`: `InventoryManager`, `EquipmentManager`) and is never unloaded.
- **3D Explore Mode** uses one scene per level/chapter (currently one test scene, `TestExplore`).
- **AR Mode** uses a **single reusable AR scene** (`AR_Session`), with combat/NPC/item content swapped in based on context (rather than a separate AR scene per interaction type). This keeps AR session/tracking initialization consistent and avoids re-establishing tracking on every mode switch. Only the combat sub-mode is implemented so far.
- Mode switching is handled by `GameModeController`, which additively loads/unloads scenes via `SceneLoader` and passes a `GameContext` payload (requested mode, enemy/weapon id, return scene) between modes.
- Namespaces mirror the folder structure under `Assets/_Project/Scripts/`: `Scar.Core`, `Scar.Exploration`, `Scar.AR` (with `Scar.AR.Combat` implemented; `Scar.AR.Dialogue`, `Scar.AR.ItemInteraction` still TBD), `Scar.Inventory`, `Scar.SaveSystem`, `Scar.Narrative` (TBD), `Scar.Data`, `Scar.UI` (TBD), `Scar.Utilities` (TBD).
- `Scar.Core` has no dependencies on other gameplay namespaces; everything else depends on Core, never the reverse. Cross-system communication goes through the event bus or interfaces (`ISaveable`, `IWeaponController`), not direct references between gameplay namespaces.

## Development status / MVP roadmap

Vertical slice — one level, one chapter, one enemy, full loop — is functionally complete end to end in Play Mode:

- [x] Phase 0 — Project setup: folder/namespace structure, Bootstrap scene with placeholder Core managers
- [x] Phase 1 — `GameModeController` + `SceneLoader`: additive load/unload between 3D and AR scenes with `GameContext` payload
- [x] Phase 2 — Minimal 3D Explore mode: third-person controller, camera, test area, encounter trigger
- [x] Phase 3 — AR session scene with plane/world tracking wiring, combat sub-mode loading on encounter *(tracking itself unverified — needs device)*
- [x] Phase 4 — One melee weapon (tap) and one ranged weapon (press-hold-release) working against a test enemy
- [x] Phase 5 — Combat reward → return to 3D mode
- [x] Phase 6 — Save/load with one chapter checkpoint, extended to also persist item pickups/objective-style flags via `PersistentFlags`
- [x] Phase 7 — Minimal inventory/equipment: equip a weapon (currently via debug trigger, not UI), combat scene picks the matching weapon controller

**Not yet done / explicitly deferred:**
- Real device validation of AR tracking (plane detection, session stability) — Play Mode cannot verify this
- Equip UI/flow (currently a placeholder debug call, not a designed interaction)
- Objective system (flags exist via `PersistentFlags`, but no dedicated `ObjectiveManager`/UI)
- NPC dialogue, item interaction AR sub-modes
- Armor slots / equipment stat model beyond weapons
- Multiple levels/chapters, real level content, actual Sangihe folklore narrative

## Open design questions

- **Free-aim vs. assisted-aim for ranged weapons** — resolved for now as assisted aim (screen-space raycast), implemented in `RangedWeaponController`. Revisit if it doesn't feel right in a device test.
- NPC dialogue and item-interaction AR scene design (not yet started)
- Armor/equipment stat model
- **Save data format** — resolved as JSON via `JsonUtility`, unencrypted, written to `Application.persistentDataPath`. Encryption can be added later by wrapping `SaveManager`'s read/write calls.
- **Whether chapter checkpoints map to sub-areas within a level scene, or separate scenes per chapter** — still open. Current `CheckpointManager` only stores a scene name + chapter id string; it doesn't yet address sub-area state within a single scene.
- Pickup → equip flow: should picking up a weapon auto-equip it, or require a separate UI action? Currently manual/debug-only.