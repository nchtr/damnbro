using UnityEngine;
using Damnbro.Player;

namespace Damnbro.World
{
    public class PickupInteractable : MonoBehaviour, IInteractable
    {
        public enum PickupType { Health, Ammo }
        public PickupType type = PickupType.Health;
        public float amount = 25f;
        public string promptText = "Pick Up [E]";
        public bool autoPickup = true;

        public string Prompt => promptText;

        public bool CanInteract(GameObject interactor) => true;

        public void Interact(GameObject interactor)
        {
            if (type == PickupType.Health)
            {
                var hp = interactor.GetComponentInParent<HealthSystem>();
                hp?.Heal(amount);
            }
            Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!autoPickup) return;
            if (other.GetComponentInParent<HealthSystem>() != null) Interact(other.gameObject);
        }
    }
}
