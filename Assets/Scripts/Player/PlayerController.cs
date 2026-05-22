using UnityEngine;

namespace Damnbro.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Move")]
        public float walkSpeed = 9f;
        public float airControl = 0.35f;
        public float acceleration = 60f;
        public float groundFriction = 12f;
        public float gravity = -28f;

        [Header("Jump")]
        public float jumpVelocity = 10f;
        public float coyoteTime = 0.12f;
        public float jumpBuffer = 0.12f;

        [Header("Dash")]
        public int maxDashCharges = 3;
        public float dashSpeed = 28f;
        public float dashDuration = 0.18f;
        public float dashRechargeTime = 1.2f;

        [Header("Slide")]
        public float slideSpeed = 16f;
        public float slideDuration = 0.9f;
        public float slideHeight = 0.9f;
        public float standHeight = 1.8f;

        [Header("Slam")]
        public float slamSpeed = 40f;
        public float slamMinAirTime = 0.05f;

        [Header("Wall Jump")]
        public float wallCheckDistance = 0.7f;
        public float wallJumpHorizontal = 12f;
        public float wallJumpVertical = 10f;
        public LayerMask wallMask = ~0;

        public bool IsGrounded { get; private set; }
        public bool IsSliding { get; private set; }
        public bool IsDashing { get; private set; }
        public bool IsSlamming { get; private set; }
        public int DashChargesRemaining { get; private set; }
        public Vector3 Velocity => _velocity;

        CharacterController _cc;
        Transform _camera;
        Vector3 _velocity;
        Vector2 _moveInput;
        float _lastGroundedTime;
        float _lastJumpPressed = -10f;
        float _slideEndTime;
        float _dashEndTime;
        Vector3 _dashDirection;
        float[] _dashCooldowns;
        float _airborneSince;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _camera = GetComponentInChildren<Camera>()?.transform;
            _dashCooldowns = new float[maxDashCharges];
            DashChargesRemaining = maxDashCharges;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;
        public void QueueJump() => _lastJumpPressed = Time.time;

        public void TryDash()
        {
            if (IsDashing || DashChargesRemaining <= 0) return;
            Vector3 dir = TransformInputToWorld(_moveInput);
            if (dir.sqrMagnitude < 0.01f) dir = transform.forward;
            _dashDirection = dir.normalized;
            _dashEndTime = Time.time + dashDuration;
            IsDashing = true;
            ConsumeDashCharge();
        }

        public void TrySlide()
        {
            if (IsSliding || !IsGrounded) return;
            IsSliding = true;
            _slideEndTime = Time.time + slideDuration;
            _cc.height = slideHeight;
            _cc.center = new Vector3(0, slideHeight * 0.5f, 0);
            Vector3 forward = TransformInputToWorld(_moveInput);
            if (forward.sqrMagnitude < 0.01f) forward = transform.forward;
            _velocity.x = forward.normalized.x * slideSpeed;
            _velocity.z = forward.normalized.z * slideSpeed;
        }

        public void TrySlam()
        {
            if (IsGrounded || IsSlamming) return;
            if (Time.time - _airborneSince < slamMinAirTime) return;
            IsSlamming = true;
            _velocity.y = -slamSpeed;
        }

        void Update()
        {
            UpdateDashCooldowns();

            bool wasGrounded = IsGrounded;
            IsGrounded = _cc.isGrounded;
            if (IsGrounded)
            {
                _lastGroundedTime = Time.time;
                if (!wasGrounded) OnLand();
            }
            else if (wasGrounded)
            {
                _airborneSince = Time.time;
            }

            if (IsDashing && Time.time >= _dashEndTime) IsDashing = false;
            if (IsSliding && (Time.time >= _slideEndTime || !IsGrounded)) EndSlide();

            if (IsDashing) DoDashMovement();
            else DoNormalMovement();

            HandleJump();

            _cc.Move(_velocity * Time.deltaTime);
        }

        void DoDashMovement()
        {
            _velocity = _dashDirection * dashSpeed;
        }

        void DoNormalMovement()
        {
            Vector3 wish = TransformInputToWorld(_moveInput) * walkSpeed;
            float control = IsGrounded ? 1f : airControl;
            Vector3 horizontal = new(_velocity.x, 0, _velocity.z);
            horizontal = Vector3.MoveTowards(horizontal, wish, acceleration * control * Time.deltaTime);

            if (IsGrounded && !IsSliding && wish.sqrMagnitude < 0.01f)
            {
                horizontal = Vector3.MoveTowards(horizontal, Vector3.zero, groundFriction * Time.deltaTime);
            }

            _velocity.x = horizontal.x;
            _velocity.z = horizontal.z;

            if (!IsGrounded) _velocity.y += gravity * Time.deltaTime;
            else if (_velocity.y < 0) _velocity.y = -2f;
        }

        void HandleJump()
        {
            bool buffered = Time.time - _lastJumpPressed <= jumpBuffer;
            bool coyote = Time.time - _lastGroundedTime <= coyoteTime;
            if (!buffered) return;

            if (coyote && !IsDashing)
            {
                _velocity.y = jumpVelocity;
                _lastJumpPressed = -10f;
                _lastGroundedTime = -10f;
                if (IsSliding) EndSlide();
                return;
            }

            if (TryWallJump()) _lastJumpPressed = -10f;
        }

        bool TryWallJump()
        {
            if (IsGrounded) return false;
            Vector3[] dirs = { transform.right, -transform.right, transform.forward, -transform.forward };
            foreach (Vector3 d in dirs)
            {
                if (Physics.Raycast(transform.position + Vector3.up, d, out RaycastHit hit, wallCheckDistance, wallMask))
                {
                    Vector3 away = hit.normal;
                    _velocity = away * wallJumpHorizontal + Vector3.up * wallJumpVertical;
                    return true;
                }
            }
            return false;
        }

        void OnLand()
        {
            if (IsSlamming)
            {
                IsSlamming = false;
                _velocity.y = jumpVelocity * 0.4f;
            }
        }

        void EndSlide()
        {
            IsSliding = false;
            _cc.height = standHeight;
            _cc.center = new Vector3(0, standHeight * 0.5f, 0);
        }

        Vector3 TransformInputToWorld(Vector2 input)
        {
            Vector3 forward = _camera != null ? _camera.forward : transform.forward;
            forward.y = 0; forward.Normalize();
            Vector3 right = _camera != null ? _camera.right : transform.right;
            right.y = 0; right.Normalize();
            return forward * input.y + right * input.x;
        }

        void ConsumeDashCharge()
        {
            for (int i = 0; i < _dashCooldowns.Length; i++)
            {
                if (_dashCooldowns[i] <= 0f)
                {
                    _dashCooldowns[i] = dashRechargeTime;
                    DashChargesRemaining--;
                    return;
                }
            }
        }

        void UpdateDashCooldowns()
        {
            int charges = 0;
            for (int i = 0; i < _dashCooldowns.Length; i++)
            {
                if (_dashCooldowns[i] > 0f)
                {
                    _dashCooldowns[i] -= Time.deltaTime;
                    if (_dashCooldowns[i] <= 0f) charges++;
                }
                else charges++;
            }
            DashChargesRemaining = charges;
        }
    }
}
