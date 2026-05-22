using UnityEngine;

namespace Damnbro.World
{
    public interface IInteractable
    {
        string Prompt { get; }
        bool CanInteract(GameObject interactor);
        void Interact(GameObject interactor);
    }
}
