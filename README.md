# SCAR — Sangihe Chronicles AR (working title)

An interactive, story-driven AR mobile game inspired by folklore and legends of the Sangihe Archipelago. The player alternates between third-person 3D exploration and AR-based combat/interaction as they move through story levels and chapters.

> Design details evolve as development progresses — this README reflects the current state of the design. See `/docs` (if present) or conversation history for deeper rationale on any decision below.

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

- **Inventory system** — holds items collected during exploration.
- **Equipment/armor system** — equippable gear affecting the player character.
- **Pickup/equip system** — connects world items to the inventory and equipment systems.
- **Save/load system** — checkpoint-based, tied to chapter progression; supports quicksave and manual load.
- **AR combat** — weapon-conditioned combat behavior:
  - **Melee weapons** (sword, spear): player must close the AR-tracked distance to within weapon range, then tap the enemy to attack.
  - **Ranged weapons** (bow, slingshot, throwable): press-and-hold to aim, release to fire.
  - Combat behavior is selected at runtime based on the player's equipped weapon (strategy pattern), not hardcoded per encounter.

## Architecture overview

- A **persistent Bootstrap scene** holds core managers (`Scar.Core`: game mode controller, scene loader, event bus, save manager) and is never unloaded.
- **3D Explore Mode** uses one scene per level/chapter.
- **AR Mode** uses a **single reusable AR scene**, with combat/NPC/item content swapped in based on context (rather than a separate AR scene per interaction type). This keeps AR session/tracking initialization consistent and avoids re-establishing tracking on every mode switch.
- Mode switching is handled by a `GameModeController` that additively loads/unloads scenes and passes a `GameContext` payload (e.g. which enemy, which weapon) between modes.
- Namespaces mirror the folder structure under `Assets/_Project/Scripts/`: `Scar.Core`, `Scar.Exploration`, `Scar.AR` (with `Scar.AR.Combat`, `Scar.AR.Dialogue`, `Scar.AR.ItemInteraction` as sub-namespaces), `Scar.Inventory`, `Scar.SaveSystem`, `Scar.Narrative`, `Scar.Data`, `Scar.UI`, `Scar.Utilities`.
- `Scar.Core` has no dependencies on other gameplay namespaces; everything else depends on Core, never the reverse. Cross-system communication goes through the event bus or interfaces (`ISaveable`, `IInteractable`, `IWeaponController`), not direct references between gameplay namespaces — this is what keeps the AR scene reusable across interaction types.

## Development status / MVP roadmap

Building a vertical slice first — one level, one chapter, one enemy, full loop — before expanding content or systems:

- [ ] Phase 0 — Project setup: folder/namespace structure, Bootstrap scene with placeholder Core managers
- [ ] Phase 1 — `GameModeController` + `SceneLoader`: additive load/unload between 3D and AR scenes with `GameContext` payload
- [ ] Phase 2 — Minimal 3D Explore mode: third-person controller, camera, test area, encounter trigger
- [ ] Phase 3 — AR session scene with working plane/world tracking, combat sub-mode loading on encounter
- [ ] Phase 4 — One melee weapon (tap) and one ranged weapon (press-hold-release) working against a test enemy
- [ ] Phase 5 — Combat reward → return to 3D mode
- [ ] Phase 6 — Save/load with one chapter checkpoint
- [ ] Phase 7 — Minimal inventory/equipment: equip a weapon, combat scene picks the matching weapon controller

## Open design questions

- Free-aim vs. assisted-aim for ranged weapon targeting
- NPC dialogue and item-interaction AR scene design (not yet started)
- Armor/equipment stat model
- Save data format (JSON vs. binary) and whether encryption is needed
- Whether chapter checkpoints map to sub-areas within a level scene, or separate scenes per chapter

## Coding conventions

- Each system lives in its own namespace under `Scar.*`, matching its folder.
- Code favors token efficiency: no excessive comments, comments only where genuinely non-obvious.
- Scene/asset wiring (component assignment, prefab setup, etc.) is done manually in-editor by the developer — scripts are written to be attached and configured, not to self-wire via generated `.meta`/scene edits.