using UnityEngine;

namespace Game.Environment
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _orientation;
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _originalTarget;
        [SerializeField] private Transform _crouchingTarget;
        [SerializeField] private float _bobbingSpeed;
        [SerializeField] private float _bobbingIntensityX;
        [SerializeField] private float _bobbingIntensityY;
        [SerializeField] private float _crouchingDelta;
        [SerializeField] private float _sensitivity = 400f;

        private readonly float _maxRotation = 90f;

        private Vector3 _shift;
        private float _bobbing;
        private float _xRotation;
        private float _yRotation;

        public void SetLookShift(Vector2 lookShift)
        {
            var lookX = lookShift.x * Time.deltaTime * _sensitivity;
            var lookY = lookShift.y * Time.deltaTime * _sensitivity;

            _yRotation += lookX;
            _xRotation -= lookY;
            _xRotation = Mathf.Clamp(_xRotation, -_maxRotation, _maxRotation);
            _transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            _orientation.rotation = Quaternion.Euler(0f, _yRotation, 0f);
        }

        public void UpdateOffset(Vector2 inputDirection, bool isCrouching, float deltaTime)
        {
            SetCrouching(isCrouching, deltaTime);
            ApplyBobbing(inputDirection, deltaTime);
        }

        private void ApplyBobbing(Vector2 inputDirection, float deltaTime)
        {
            if (inputDirection == Vector2.zero)
            {
                return;
            }

            _bobbing += deltaTime * _bobbingSpeed;
            var sinY = -Mathf.Abs(Mathf.Sin(_bobbing) * _bobbingIntensityY);
            var sinX = _orientation.right * _bobbingIntensityX * Mathf.Cos(_bobbing);
            _shift = Vector3.up * sinY;
            _shift += sinX;
        }

        private void SetCrouching(bool isCrouching, float deltaTime)
        {
            var delta = _crouchingDelta * deltaTime;
            if (isCrouching)
            {
                _target.position = Vector3.MoveTowards(_target.position, _crouchingTarget.position + _shift, delta);
            }
            else
            {
                _target.position = Vector3.MoveTowards(_target.position, _originalTarget.position + _shift, delta);
            }
        }
    }
}
