using UnityEngine;

namespace Damnbro.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Look")]
        public float mouseSensitivity = 2.2f;
        public float pitchMin = -85f;
        public float pitchMax = 85f;

        [Header("Effects")]
        public float fovBase = 95f;
        public float fovDashBoost = 12f;
        public float fovLerp = 10f;
        public float tiltAngle = 6f;
        public float tiltLerp = 8f;

        public Transform yawRoot;
        public Transform pitchRoot;
        public PlayerController playerRef;

        Camera _cam;
        float _yaw;
        float _pitch;
        float _currentTilt;
        Vector2 _lookInput;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            if (yawRoot == null) yawRoot = transform.parent;
            if (pitchRoot == null) pitchRoot = transform;
            if (_cam != null) _cam.fieldOfView = fovBase;
        }

        public void SetLookInput(Vector2 delta) => _lookInput = delta;

        void Update()
        {
            _yaw += _lookInput.x * mouseSensitivity;
            _pitch -= _lookInput.y * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            if (yawRoot != null) yawRoot.rotation = Quaternion.Euler(0, _yaw, 0);
            if (pitchRoot != null) pitchRoot.localRotation = Quaternion.Euler(_pitch, 0, _currentTilt);

            float targetTilt = 0f;
            if (playerRef != null)
            {
                Vector3 local = yawRoot != null ? yawRoot.InverseTransformDirection(playerRef.Velocity) : playerRef.Velocity;
                targetTilt = -Mathf.Clamp(local.x / 12f, -1f, 1f) * tiltAngle;
            }
            _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, tiltLerp * Time.deltaTime);

            if (_cam != null && playerRef != null)
            {
                float targetFov = fovBase + (playerRef.IsDashing ? fovDashBoost : 0f);
                _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFov, fovLerp * Time.deltaTime);
            }
        }
    }
}
