using UnityEngine;
using Damnbro.Weapons;

namespace Damnbro.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerController controller;
        public PlayerCamera cameraRig;
        public PlayerInteraction interaction;
        public WeaponManager weapons;
        public Parry parry;

        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode dashKey = KeyCode.LeftShift;
        public KeyCode slideKey = KeyCode.LeftControl;
        public KeyCode slamKey = KeyCode.C;
        public KeyCode interactKey = KeyCode.E;
        public KeyCode reloadKey = KeyCode.R;
        public KeyCode parryKey = KeyCode.F;

        void Update()
        {
            if (controller != null)
            {
                Vector2 move = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                controller.SetMoveInput(move);
                if (Input.GetKeyDown(jumpKey)) controller.QueueJump();
                if (Input.GetKeyDown(dashKey)) controller.TryDash();
                if (Input.GetKeyDown(slideKey)) controller.TrySlide();
                if (Input.GetKeyDown(slamKey)) controller.TrySlam();
            }

            if (cameraRig != null)
            {
                Vector2 look = new(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
                cameraRig.SetLookInput(look);
            }

            if (interaction != null && Input.GetKeyDown(interactKey)) interaction.TryInteract();
            if (parry != null && Input.GetKeyDown(parryKey)) parry.TryStrike();

            if (weapons != null)
            {
                if (Input.GetMouseButton(0)) weapons.TriggerFire();
                if (Input.GetMouseButtonDown(1)) weapons.TriggerAltFire();
                if (Input.GetKeyDown(reloadKey)) weapons.TriggerReload();
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll > 0.05f) weapons.CycleNext();
                else if (scroll < -0.05f) weapons.CyclePrev();
                for (int i = 1; i <= 9; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0 + i)) weapons.SelectSlot(i - 1);
                }
            }
        }
    }
}
