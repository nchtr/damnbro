# Assembly & configuration

The demo scene at `Assets/Scenes/Demo.unity` ships with everything wired:

- Directional light + ground plane
- Player rig: CharacterController, `PlayerController`, `PlayerInput`, `PlayerInteraction`, `HealthSystem`, `WeaponManager`, `Parry`
- A blue capsule **PlayerModel** child as the player visual (collider stripped)
- `CameraRig` (Camera + AudioListener + `PlayerCamera`) and a `WeaponHolder` child
- Three weapons under `WeaponHolder` (already in `WeaponManager.weapons`):
  - **Revolver** (slot 0) — `HitscanWeapon`, 25 dmg, 6-round mag
  - **Knife** (slot 1) — `MeleeWeapon`, 50 dmg, infinite mag
  - **Launcher** (slot 2) — `ProjectileWeapon` firing `Assets/Prefabs/Projectile.prefab` (radius-4 explosion)
- `GameManager` for cursor toggle and respawn
- `SceneServices` GameObject with `RuntimeNavMeshBaker`, `WaveSpawner`, and `StyleMeter`
- A `Canvas` containing the existing `HUD` (TMP text components), wired to the player

Press **Play** in the demo scene — nothing else to set up.

## Controls

| Action       | Key                       |
| ------------ | ------------------------- |
| Move         | WASD                      |
| Look         | Mouse                     |
| Jump         | Space                     |
| Dash         | Left Shift                |
| Slide        | Left Ctrl (on ground)     |
| Ground slam  | Left Ctrl (in air) or C   |
| Wall jump    | Space (next to wall)      |
| Fire         | LMB                       |
| Alt-fire     | RMB                       |
| Reload       | R                         |
| Switch weapon| Mouse wheel / 1–3         |
| Interact     | E                         |
| Parry / melee| F                         |
| Free cursor  | Esc                       |

`Ctrl` is context-sensitive — slide when grounded, slam when airborne. The dedicated `C` key still works for slam if you prefer it explicit.

## Customising

### Adjust weapons
Select `Player → CameraRig → WeaponHolder → Revolver` (or Knife/Launcher) and edit the inspector — `damage`, `fireRate`, `magazineSize`, `spreadDegrees`, etc. To add a fourth weapon, duplicate one of the three, change its component and `slot`, then drag it into `Player → WeaponManager → Weapons`.

### Tune the spawner
Select `SceneServices → WaveSpawner`. The `Waves` list is empty by default, so the spawner generates three default waves (3M, 4M+1R, 5M+2R) on Start. Override by populating the list in the inspector. You can also drop Transforms into `Spawn Points` to use fixed locations instead of the player-relative ring.

### Use prefab enemies
By default enemies are built from primitive capsules. Drag a prefab into `WaveSpawner.meleePrefab` / `rangedPrefab` to use your own.

### Wire the wave label
Add a `TMP_Text` element to the HUD canvas, then drag it into `WaveSpawner.waveLabel` on `SceneServices`. The spawner will display wave state ("Wave 2 — 4M / 1R", "Wave 1 cleared", etc.).

### Replace placeholder weapon meshes
Swap the cube mesh on each weapon GameObject (Mesh Filter component) for your imported model and resize the Transform. The collider-less placeholders won't fight your geometry.

### Replace the player capsule
Reparent your character mesh under `Player` and delete the `PlayerModel` capsule. Make sure no collider is on the visual — the `CharacterController` is the only collider that should be on the player.

### Switch to the new Input System
1. **Edit → Project Settings → Player → Other Settings → Active Input Handling** → **Both** or **Input System Package (New)**. Restart the editor.
2. On the Player GameObject, remove `PlayerInput` and add `PlayerInputNew`.
3. Drag `Assets/Input/Controls.inputactions` into `PlayerInputNew.Actions`.
4. Re-wire `Controller`, `Camera Rig`, `Interaction`, `Weapons`, `Parry`.

## Common pitfalls

- **Pink materials:** URP needs its global render-pipeline asset assigned. Open **Project Settings → Graphics → Scriptable Render Pipeline Settings** and pick a URP Asset (Unity's URP package ships one).
- **Hitscan shots invisible:** `HitscanWeapon` draws a thin yellow tracer automatically (built-in `Sprites/Default` material). If you see nothing, the shader is missing in your build profile — assign a `LineRenderer` prefab to the weapon's `Tracer Prefab` field as a fallback.
- **Launcher rocket detonates on the player:** make sure the Launcher's `Muzzle` transform isn't inside the CharacterController. The default scene puts it at the cube origin, slightly in front of the camera, which is safe.
- **Enemies stand still:** the NavMesh is rebuilt at Awake by `RuntimeNavMeshBaker`. If you've made the ground larger than 80×80, raise `Bounds Size` on that component.
- **No HUD updates:** the `HUD` component on the `UI` Canvas needs the `Health`, `Player`, `Interaction`, `Weapons`, and `Style` source slots wired to the Player components and the SceneServices StyleMeter. The default scene does this — if you re-create the scene, redo it.
