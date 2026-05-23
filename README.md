# damnbro

A ULTRAKILL-inspired boomer-shooter project template for Unity. Focuses on the
movement feel that defines the genre: fast ground speed, multi-charge dash,
slide, ground slam, wall jump, and a swappable weapon system with hitscan and
projectile archetypes. Also includes a raycast interaction system, a NavMesh
enemy base class, and a style-meter scaffold.

This repo contains:

- C# gameplay scripts under `Assets/Scripts/`
- A fully-wired demo scene at `Assets/Scenes/Demo.unity` — player rig, camera, ground, light, GameManager, player capsule visual, three weapon GameObjects (Revolver / Knife / Launcher), HUD canvas, and a `SceneServices` object with the NavMesh baker, wave spawner, and style meter
- `Assets/Prefabs/Projectile.prefab` — the rocket the Launcher fires
- `Assets/Input/Controls.inputactions` for the new Input System variant
- Project settings with the `Enemy` layer + `Player` tag configured
- Unity package manifest pinned to URP + Cinemachine + TMP + Input System + `com.unity.ai.navigation`
- A `ProjectVersion.txt` targeting **Unity 2022.3 LTS**

Just open the scene and press **Play**. See [SETUP.md](SETUP.md) for what to customise.

## Requirements

- Unity Hub
- Unity **2022.3.20f1** (or another 2022.3 LTS patch — open the project and let Hub upgrade)
- A graphics card capable of URP

## Quick start

1. Clone the repo.
2. In Unity Hub click **Add → Add project from disk** and pick this folder.
3. Open the project. Unity will generate `Library/`, `Logs/`, `Temp/`, `UserSettings/` and resolve packages — first import takes 1–3 minutes.
4. Follow [SETUP.md](SETUP.md) to assemble the demo scene (≈10 minutes).
5. Press **Play**.

## Default controls

| Action       | Key                 |
| ------------ | ------------------- |
| Move         | WASD                |
| Look         | Mouse               |
| Jump         | Space               |
| Dash         | Left Shift          |
| Slide        | Left Ctrl           |
| Ground slam  | C (mid-air)         |
| Wall jump    | Space (next to wall)|
| Fire         | LMB                 |
| Alt-fire     | RMB                 |
| Reload       | R                   |
| Switch wpn   | Mouse wheel / 1–3   |
| Interact     | E                   |
| Parry / melee| F                   |
| Slide / slam | Left Ctrl (context) |
| Free cursor  | Esc                 |

## Script overview

```
Assets/Scripts/
  Core/        GameManager, RuntimeNavMeshBaker, WaveSpawner, EnemyFactory
  Player/      PlayerController, PlayerCamera, PlayerInput, PlayerInputNew, PlayerInteraction, HealthSystem, Parry
  Weapons/     WeaponManager, WeaponBase, HitscanWeapon, MeleeWeapon, ProjectileWeapon, Projectile
  Enemies/     EnemyBase, EnemyMelee, EnemyRanged
  World/       IInteractable, DoorInteractable, PickupInteractable
  UI/          HUD, StyleMeter
```

All scripts live in the `Damnbro.*` namespaces.

## Input handling

Two interchangeable drivers are included:

- **`PlayerInput`** — uses Unity's legacy axes (`Horizontal`, `Vertical`, `Mouse X/Y`). Works out of the box; this is what the demo scene wires up.
- **`PlayerInputNew`** — uses the new Input System with `Assets/Input/Controls.inputactions`. Enable by removing `PlayerInput` from the Player and adding `PlayerInputNew` with the asset dragged into the `Actions` field. (The class is wrapped in `#if ENABLE_INPUT_SYSTEM`, so it stays inert until the Input System package's Active Input Handling is set to **Both** or **Input System Package**.)

## Extending

- **New weapon:** subclass `WeaponBase` and override `Fire()` (and optionally `AltFire()`). Add a GameObject under `Player → CameraRig → WeaponHolder` with the component, then drag it into `WeaponManager.weapons` on the Player.
- **New enemy:** subclass `EnemyBase` and override `Attack()`. See `EnemyRanged` for a projectile-firing example and `EnemyFactory` for how to spin one up from primitives.
- **New interactable:** implement `IInteractable` and put a collider on the object so the player's interaction raycast can hit it.
- **Parry:** `Parry.cs` does a `SphereCastAll` along camera-forward. It calls `Projectile.Reflect(player)` on any projectile it sweeps, which flips ownership and damage, so enemy shots fly back and hurt them.
- **Waves:** edit the `WaveSpawner` on the `SceneServices` GameObject. Empty list = three default waves; populated list = your own schedule.

## License

See [LICENSE](LICENSE).
