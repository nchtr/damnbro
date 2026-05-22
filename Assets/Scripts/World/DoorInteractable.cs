using UnityEngine;

namespace Damnbro.World
{
    public class DoorInteractable : MonoBehaviour, IInteractable
    {
        public string promptOpen = "Open Door [E]";
        public string promptClose = "Close Door [E]";
        public Vector3 openLocalEuler = new(0, 90, 0);
        public float lerpSpeed = 6f;

        bool _isOpen;
        Quaternion _closedRot;
        Quaternion _openRot;

        public string Prompt => _isOpen ? promptClose : promptOpen;

        void Awake()
        {
            _closedRot = transform.localRotation;
            _openRot = _closedRot * Quaternion.Euler(openLocalEuler);
        }

        void Update()
        {
            Quaternion target = _isOpen ? _openRot : _closedRot;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, lerpSpeed * Time.deltaTime);
        }

        public bool CanInteract(GameObject interactor) => true;
        public void Interact(GameObject interactor) => _isOpen = !_isOpen;
    }
}
