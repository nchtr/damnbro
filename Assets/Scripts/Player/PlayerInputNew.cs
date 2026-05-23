#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using Damnbro.Weapons;

namespace Damnbro.Player
{
    public class PlayerInputNew : MonoBehaviour
    {
        [Header("Refs")]
        public PlayerController controller;
        public PlayerCamera cameraRig;
        public PlayerInteraction interaction;
        public WeaponManager weapons;
        public Parry parry;

        [Header("Input Actions")]
        public InputActionAsset actions;

        InputAction _move;
        InputAction _look;
        InputAction _jump;
        InputAction _dash;
        InputAction _slide;
        InputAction _slam;
        InputAction _interact;
        InputAction _reload;
        InputAction _parry;
        InputAction _fire;
        InputAction _altFire;
        InputAction _cycleNext;
        InputAction _cyclePrev;
        InputAction[] _slotSelectors = new InputAction[9];

        void OnEnable()
        {
            if (actions == null) { enabled = false; return; }
            actions.Enable();

            var map = actions.FindActionMap("Player", throwIfNotFound: true);
            _move = map.FindAction("Move", true);
            _look = map.FindAction("Look", true);
            _jump = map.FindAction("Jump", true);
            _dash = map.FindAction("Dash", true);
            _slide = map.FindAction("Slide", true);
            _slam = map.FindAction("Slam", true);
            _interact = map.FindAction("Interact", true);
            _reload = map.FindAction("Reload", true);
            _parry = map.FindAction("Parry", true);
            _fire = map.FindAction("Fire", true);
            _altFire = map.FindAction("AltFire", true);
            _cycleNext = map.FindAction("CycleNext", true);
            _cyclePrev = map.FindAction("CyclePrev", true);
            for (int i = 0; i < 9; i++)
                _slotSelectors[i] = map.FindAction($"Slot{i + 1}", false);

            _jump.performed += _ => controller?.QueueJump();
            _dash.performed += _ => controller?.TryDash();
            _slide.performed += _ => { if (controller == null) return; if (controller.IsGrounded) controller.TrySlide(); else controller.TrySlam(); };
            _slam.performed += _ => controller?.TrySlam();
            _interact.performed += _ => interaction?.TryInteract();
            _reload.performed += _ => weapons?.TriggerReload();
            _parry.performed += _ => parry?.TryStrike();
            _altFire.performed += _ => weapons?.TriggerAltFire();
            _cycleNext.performed += _ => weapons?.CycleNext();
            _cyclePrev.performed += _ => weapons?.CyclePrev();
            for (int i = 0; i < 9; i++)
            {
                int slot = i;
                if (_slotSelectors[i] != null) _slotSelectors[i].performed += _ => weapons?.SelectSlot(slot);
            }
        }

        void OnDisable()
        {
            if (actions != null) actions.Disable();
        }

        void Update()
        {
            if (controller != null) controller.SetMoveInput(_move.ReadValue<Vector2>());
            if (cameraRig != null) cameraRig.SetLookInput(_look.ReadValue<Vector2>());
            if (weapons != null && _fire.IsPressed()) weapons.TriggerFire();
        }
    }
}
#endif
