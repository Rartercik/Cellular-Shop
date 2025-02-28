using UnityEngine;

namespace Game.PlayerComponents
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _orientation;
        [SerializeField] private Collider _collider;
        [SerializeField] private AudioSource _footSteps;
        [SerializeField] private float _jumpForce = 1f;
        [SerializeField] private float _movementForce = 1f;
        [SerializeField] private float _maxVelocity = 1f;
        [SerializeField] private float _gravity = 9.81f;
        [SerializeField] private float _crouchMultiplier = 1f;
        [SerializeField] private float _airMultiplier;
        [SerializeField] private float _groundDrag;

        private readonly float _jumpCheckDistance = 0.01f;

        private Vector2 _inputDirection;
        private bool _tryJump;
        private bool _tryCrouch;

        public bool IsCrouching { get; private set; }

        private void FixedUpdate()
        {
            var onFloor = CheckOnFloor(_collider, _jumpCheckDistance);
            IsCrouching = onFloor && _tryCrouch;
            SetLinearDamping(_rigidbody, onFloor);
            ApplyGravity(_rigidbody, _gravity);
            Move(_rigidbody, _orientation, _inputDirection, onFloor, IsCrouching, _movementForce, _crouchMultiplier, _airMultiplier);
            ControlVelocity(_rigidbody, IsCrouching, _maxVelocity, _crouchMultiplier);
            TryJump(_rigidbody, _tryJump, onFloor, _jumpForce);
            PlayFootSteps(_footSteps, _inputDirection, onFloor);
        }

        public void SetInputDirection(Vector2 inputDirection)
        {
            _inputDirection = inputDirection;
        }

        public void SetJumpAttempt(bool tryJump)
        {
            _tryJump = tryJump;
        }

        public void SetCrouchAttempt(bool tryCrouch)
        {
            _tryCrouch = tryCrouch;
        }

        private void SetLinearDamping(Rigidbody rigidbody, bool onFloor)
        {
            if (onFloor)
            {
                rigidbody.linearDamping = _groundDrag;
            }
            else
            {
                rigidbody.linearDamping = 0f;
            }
        }

        private void ApplyGravity(Rigidbody rigidbody, float gravity)
        {
            rigidbody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }

        private void Move(Rigidbody rigidbody, Transform orientation, Vector2 inputDirection, bool onFloor, bool isCrouching,
            float movementForce, float crouchMultiplier, float airMultiplier)
        {
            var direction = orientation.rotation * new Vector3(inputDirection.x, 0f, inputDirection.y);
            var force = direction * _movementForce;
            if (onFloor == false)
            {
                force *= airMultiplier;
            }
            if (isCrouching)
            {
                force *= crouchMultiplier;
            }
            rigidbody.AddForce(force);
        }

        private void ControlVelocity(Rigidbody rigidbody, bool isCrouching, float maxVelocity, float crouchMultiplier)
        {
            if (isCrouching) maxVelocity *= crouchMultiplier;

            var velocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
            if (velocity.magnitude > maxVelocity)
            {
                velocity = velocity.normalized * maxVelocity;
            }
            velocity.y = rigidbody.linearVelocity.y;
            rigidbody.linearVelocity = velocity;
        }

        private void TryJump(Rigidbody rigidbody, bool tryJump, bool onFloor, float jumpForce)
        {
            if (tryJump && onFloor)
            {
                Jump(rigidbody, jumpForce);
            }
        }

        private void Jump(Rigidbody rigidbody, float jumpForce)
        {
            rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private bool CheckOnFloor(Collider body, float jumpCheckDistance)
        {
            return Physics.Raycast(body.bounds.center, Vector3.down, body.bounds.extents.y + jumpCheckDistance);
        }

        private void PlayFootSteps(AudioSource audioSource, Vector2 inputDirection, bool onFloor)
        {
            if (inputDirection == Vector2.zero || onFloor == false)
            {
                audioSource.Pause();
                return;
            }
            audioSource.UnPause();
        }
    }
}
