# Assembly & configuration

The demo scene at `Assets/Scenes/Demo.unity` already contains:

- Directional light + ground plane (on the `Ground` layer)
- A complete Player rig (CharacterController, `PlayerController`, `PlayerInput`, `PlayerInteraction`, `HealthSystem`, `WeaponManager`, `Parry`)
- A child `CameraRig` (Camera + AudioListener + `PlayerCamera` script), with all script references already wired
- A grandchild empty `WeaponHolder`
- A `GameManager` GameObject with the player's `HealthSystem` already referenced

`ProjectSettings/TagManager.asset` is pre-populated with the `Player` tag and the `Player` / `Enemy` / `Interactable` / `Ground` layers. `InputManager.asset` ships with the default legacy axes that `PlayerInput.cs` reads.

So once Unity finishes its first import, **press Play in `Demo.unity` and you'll already have movement, looking, dashing, sliding, slamming, wall-jumping, and parry — there just aren't any enemies or weapons yet.**

## 0. Open the project

1. Launch Unity Hub → **Add → Add project from disk** → pick this folder.
2. If Hub prompts you to install Unity 2022.3.20f1, accept (any 2022.3.x LTS works).
3. Open the project. Wait for the package resolver and asset import to finish.
4. Open `Assets/Scenes/Demo.unity` (already in the build settings as scene 0).

## 1. Add a weapon

1. In the Hierarchy, find `Player → CameraRig → WeaponHolder`.
2. Right-click `WeaponHolder` → **Create Empty**, name it `Revolver`.
3. Add component **HitscanWeapon**. Configure:
   - `View Camera` → drag the `CameraRig` Camera component
   - `Muzzle` → for now, drag `WeaponHolder` itself (you can refine with a barrel-tip empty later)
   - `Hit Mask` → set to everything **except** the `Player` layer
   - `Damage` 25 · `Range` 200 · `Fire Rate` 4 · `Magazine Size` 6
4. Select `Player`, find the **Weapon Manager** component, expand `Weapons`, drag the `Revolver` GameObject into the list.

Add more (`Shotgun` via `HitscanWeapon` with `Pellets = 8`, `Spread Degrees = 5`; `Launcher` via `ProjectileWeapon` with a projectile prefab) the same way and bump their `Slot` values to 1, 2, etc.

### Making a projectile prefab

1. **GameObject → 3D Object → Sphere**, scale `(0.2, 0.2, 0.2)`.
2. Add **Rigidbody** (disable Use Gravity).
3. Add **Projectile** script. Set `Damage` 30, `Explosion Radius` 4 for a rocket, 0 for a bullet.
4. Drag the GameObject into `Assets/Prefabs/` (create the folder) to make a prefab, then delete the in-scene copy.
5. Assign that prefab to your `ProjectileWeapon`'s `Projectile Prefab` field.

## 2. Add enemies

1. **GameObject → 3D Object → Capsule**, position it ~10 m from the Player.
2. Set its layer to `Enemy`.
3. Add components: **NavMeshAgent**, **HealthSystem**, and either **EnemyMelee** or **EnemyRanged**.
4. For `EnemyRanged`, drag the projectile prefab from step 1 into the `Projectile Prefab` field and create an empty child as `Muzzle`.
5. **Window → AI → Navigation** → **Bake** so the agents have a NavMesh to walk on.
6. Press Play — they should chase and attack.

## 3. Add interactables

- **Door:** parent a cube to an empty pivot at the door's hinge edge. Put `DoorInteractable` on the parent. Press `E` near it to swing.
- **Pickup:** any object with a trigger collider + `PickupInteractable` (type = Health, amount = 25). With `autoPickup = true` it heals on touch; with `false` press `E`.

## 4. Add a HUD

1. **GameObject → UI → Canvas**.
2. Inside the Canvas create:
   - `Slider` named `HealthBar`
   - `TextMeshPro - Text` named `HealthLabel`, `AmmoLabel`, `DashLabel`, `PromptLabel`, `StyleRankLabel`
   - `Slider` named `StyleBar`
3. Add a `HUD` component to the Canvas root. Drag the Player's `HealthSystem`, `PlayerController`, `PlayerInteraction`, `WeaponManager` into the matching fields. Drag each UI element into its slot.
4. Optionally add a `StyleMeter` component to the Player and drag it into the HUD's `Style` field — the meter currently only gets points from successful parries; add `style.AddPoints(...)` calls wherever you want to reward kills.

## 5. (Optional) Switch to the new Input System

The demo uses the legacy `PlayerInput` script. To swap in the new Input System:

1. **Edit → Project Settings → Player → Other Settings → Active Input Handling** → set to **Both** or **Input System Package (New)** and restart the editor when prompted.
2. On the **Player** GameObject, remove `PlayerInput` and add `PlayerInputNew` instead.
3. Drag `Assets/Input/Controls.inputactions` into `PlayerInputNew`'s `Actions` field.
4. Re-wire `Controller`, `Camera Rig`, `Interaction`, `Weapons`, `Parry` (same fields as the legacy version).

The provided `Controls.inputactions` already maps WASD, mouse look, Space/Shift/Ctrl/C/E/R/F, LMB/RMB, mouse-wheel, and 1–5.

## 6. (Optional) Use Parry

The Player already has a `Parry` component wired to the camera. Press `F` to do a forward melee sweep that:

- Damages anything with a `HealthSystem` in front of the player
- **Reflects** any `Projectile` in range — it flips ownership and multiplies damage, so enemy rockets fly back at the shooter
- Awards style points if a reflect succeeds (set `Style Meter` on the Parry component to enable)

To make this more visible, assign a `Parry Fx Prefab` (e.g. a quick particle effect) to spawn at the reflect point.

## Common pitfalls

- **Camera doesn't rotate / scripts marked "missing":** Unity needs to compile the C# first. Wait until the spinner in the bottom-right of the editor stops, then re-open the scene. The wired references in the scene reference scripts by GUID, so once the scripts compile they will populate.
- **Player slides forever on slopes:** that's the slide ability while it's active. If it's happening during plain walking, raise `Ground Friction` on the `PlayerController`.
- **Hitscan never hits enemies:** make sure enemy colliders are not on the `Player` layer and `Hit Mask` on the weapon includes the `Enemy` layer.
- **Enemies don't move:** the NavMesh isn't baked, or the enemy isn't on it. Re-bake with **Window → AI → Navigation → Bake**.
- **Dash doesn't recharge:** check `Max Dash Charges` ≥ 1 and `Dash Recharge Time` > 0.
- **Materials show pink:** URP needs its global settings asset. The first time the editor opens, accept the prompt to **Create URP Asset** or use **Window → Rendering → Render Pipeline Converter** to update the built-in default material.
