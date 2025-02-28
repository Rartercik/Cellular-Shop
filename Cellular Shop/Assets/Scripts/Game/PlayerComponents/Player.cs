using UnityEngine;
using UnityEngine.InputSystem;
using Game.Environment;

namespace Game.PlayerComponents
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CameraMovement _cameraMovement;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerInteraction _interaction;
        [SerializeField] private Canvas _menuWindow;

        private PlayerInput _playerInput;
        private InputAction _menu;
        private InputAction _move;
        private InputAction _jump;
        private InputAction _crouch;
        private InputAction _interact;
        private InputAction _combine;
        private InputAction _throw;
        private InputAction _rotate;
        private InputAction _rotateLargely;
        private InputAction _choose1;
        private InputAction _choose2;
        private InputAction _choose3;
        private InputAction _zoom;
        private InputAction _look;

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _menu = _playerInput.Player.Menu;
            _move = _playerInput.Player.Move;
            _jump = _playerInput.Player.Jump;
            _crouch = _playerInput.Player.Crouch;
            _interact = _playerInput.Player.Interact;
            _combine = _playerInput.Player.Combine;
            _throw = _playerInput.Player.Throw;
            _rotate = _playerInput.Player.Rotate;
            _rotateLargely = _playerInput.Player.RotateLargely;
            _choose1 = _playerInput.Player.Choose1;
            _choose2 = _playerInput.Player.Choose2;
            _choose3 = _playerInput.Player.Choose3;
            _zoom = _playerInput.Player.Zoom;
            _look = _playerInput.Player.Look;

            _interaction.Initialize();
        }

        private void OnEnable()
        {
            _menu.Enable();
            _move.Enable();
            _jump.Enable();
            _crouch.Enable();
            _interact.Enable();
            _combine.Enable();
            _throw.Enable();
            _rotate.Enable();
            _rotateLargely.Enable();
            _choose1.Enable();
            _choose2.Enable();
            _choose3.Enable();
            _zoom.Enable();
            _look.Enable();
        }

        private void OnDisable()
        {
            _menu.Disable();
            _move.Disable();
            _jump.Disable();
            _crouch.Disable();
            _interact.Disable();
            _combine.Disable();
            _throw.Disable();
            _rotate.Disable();
            _rotateLargely.Disable();
            _choose1.Disable();
            _choose2.Disable();
            _choose3.Disable();
            _zoom.Disable();
            _look.Disable();
        }

        private void Update()
        {
            var menu = _menu.triggered;
            if (menu)
            {
                if (_menuWindow.enabled)
                {
                    _menuWindow.enabled = false;
                    Time.timeScale = 1f;
                }
                else
                {
                    _menuWindow.enabled = true;
                    Time.timeScale = 0f;
                }
            }
            var inputDirection = _move.ReadValue<Vector2>();
            var tryJump = _jump.IsPressed();
            var tryCrouch = _crouch.IsPressed();
            var tryInteract = _interact.triggered;
            var tryCombine = _combine.IsPressed();
            var tryThrow = _throw.triggered;
            var tryRotate = _rotate.triggered;
            var tryRotateLargely = _rotateLargely.triggered;
            var choose1 = _choose1.triggered;
            var choose2 = _choose2.triggered;
            var choose3 = _choose3.triggered;
            var tryZoom = _zoom.IsPressed();
            var lookShift = _look.ReadValue<Vector2>();
            _movement.SetInputDirection(inputDirection);
            _movement.SetJumpAttempt(tryJump);
            _movement.SetCrouchAttempt(tryCrouch);
            if (tryInteract) _interaction.TryInteract();
            if (tryCombine) _interaction.TryProcessCombining(Time.deltaTime);
            else _interaction.TryStopCombining();
            if (tryThrow) _interaction.TryThrowItem();
            if (tryRotateLargely) _interaction.TryRotateItemLargely();
            else if (tryRotate) _interaction.TryRotateItem();
            if (choose1) _interaction.ChooseItem(0);
            if (choose2) _interaction.ChooseItem(1);
            if (choose3) _interaction.ChooseItem(2);
            _interaction.SetZoomAttempt(tryZoom, Time.deltaTime);
            _cameraMovement.SetLookShift(lookShift);
            _cameraMovement.UpdateOffset(inputDirection, _movement.IsCrouching, Time.deltaTime);
        }
    }
}
