using UnityEngine;
using Damnbro.World;

namespace Damnbro.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        public Camera viewCamera;
        public float interactRange = 3.5f;
        public LayerMask interactMask = ~0;

        public IInteractable CurrentTarget { get; private set; }

        void Update()
        {
            CurrentTarget = null;
            if (viewCamera == null) return;
            Ray ray = new(viewCamera.transform.position, viewCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactMask, QueryTriggerInteraction.Collide))
            {
                CurrentTarget = hit.collider.GetComponentInParent<IInteractable>();
            }
        }

        public void TryInteract()
        {
            CurrentTarget?.Interact(gameObject);
        }
    }
}
