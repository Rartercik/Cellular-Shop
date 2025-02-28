using UnityEngine;
using Game.Environment;

namespace Game.PlayerComponents
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _itemHolder;
        [SerializeField] private Transform _zoomedItemHolder;
        [SerializeField] private Transform _zoomedItemTarget;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private LayerMask _interactable;
        [SerializeField] private Vector3 _throwForce;
        [SerializeField] private Vector3 _itemRotation;
        [SerializeField] private Vector3 _itemLargerRotation;
        [SerializeField] private float _grabDistance;
        [SerializeField] private float _zoomingTime;
        [SerializeField] private float _itemRotationSpeed;
        [SerializeField] private float _maxRemainingAngleToRotate = 1f;

        private Combiner _combiner;
        private Vector3 _itemHolderInitialPosition;
        private Vector3 _holdItemTargetPosition;
        private Quaternion _itemHolderInitialRotation;
        private Quaternion _holdItemTargetRotation;
        private Quaternion _targetRotation;
        private float _zoomPositionSpeed;
        private float _zoomRotationSpeed;

        private void Update()
        {
            ProceedItemRotation();
        }

        public void Initialize()
        {
            var distance = Vector3.Distance(_itemHolder.parent.position, _zoomedItemTarget.position);
            var angle = Quaternion.Angle(_itemHolder.rotation, _zoomedItemTarget.rotation);
            _zoomPositionSpeed = _zoomingTime == 0f ? float.PositiveInfinity : distance / _zoomingTime;
            _zoomRotationSpeed = _zoomingTime == 0f ? float.PositiveInfinity : angle / _zoomingTime;
            _itemHolderInitialPosition = _itemHolder.localPosition;
            _itemHolderInitialRotation = _itemHolder.localRotation;
            _holdItemTargetPosition = _zoomedItemHolder.localPosition;
            _holdItemTargetRotation = _zoomedItemHolder.localRotation;
            _targetRotation = _itemHolderInitialRotation;
        }

        public void ChooseItem(int idex)
        {
            _inventory.ChooseItem(idex);
        }

        public void TryInteract()
        {
            var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, _grabDistance, _interactable) && _inventory.Occupied == false)
            {
                if (hit.collider.TryGetComponent<Item>(out var item) == false) return;

                Grab(item, _inventory);
            }
        }

        public void TryProcessCombining(float deltaTime)
        {
            var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out var hit, _grabDistance, _interactable) && _inventory.Occupied)
            {
                if (hit.collider.TryGetComponent<Combiner>(out var combiner) == false)
                {
                    TryStopCombining();
                    return;
                }

                _inventory.TryProcessCombining(combiner, () => ProcessCombiningStart(combiner), ProcessCombiningFinish, ProcessCombiningStop, deltaTime);
            }
            else
            {
                TryStopCombining();
            }
        }

        public void TryStopCombining()
        {
            if (_combiner == null) return;

            _inventory.StopCombining(_combiner, ProcessCombiningStop);
        }

        public void TryThrowItem()
        {
            if (_inventory.Occupied == false) return;

            var throwForce = _cameraTransform.rotation * _throwForce;
            _inventory.ThrowItem(throwForce);
            _zoomedItemHolder.localPosition = _holdItemTargetPosition;
            _zoomedItemHolder.localRotation = _holdItemTargetRotation;
            ResetItemHolder();
        }

        public void SetZoomAttempt(bool tryZoom, float deltaTime)
        {
            if (_inventory.Occupied == false || _combiner != null) return;

            var targetPosition = tryZoom ? _zoomedItemTarget.localPosition : _holdItemTargetPosition;
            var targetRotation = tryZoom ? _zoomedItemTarget.localRotation : _holdItemTargetRotation;
            _zoomedItemHolder.localPosition = Vector3.MoveTowards(_zoomedItemHolder.localPosition, targetPosition, _zoomPositionSpeed * deltaTime);
            _zoomedItemHolder.localRotation = Quaternion.RotateTowards(_zoomedItemHolder.localRotation, targetRotation, _zoomRotationSpeed * deltaTime);
        }

        public void TryRotateItem()
        {
            TryRotateItem(Quaternion.Euler(_itemRotation));
        }

        public void TryRotateItemLargely()
        {
            TryRotateItem(Quaternion.Euler(_itemLargerRotation));
        }

        private void Grab(Item item, Inventory inventory)
        {
            inventory.AddItem(item, _itemHolder, _zoomingTime);
        }

        private void ProcessCombiningStart(Combiner combiner)
        {
            _combiner = combiner;
        }

        private void ProcessCombiningFinish()
        {
            if (_combiner.ExtractItemOnFinishing)
            {
                ResetItemHolder();
            }
            _combiner = null;
        }

        private void ProcessCombiningStop()
        {
            if (_combiner.ExtractItemOnExiting)
            {
                ResetItemHolder();
            }
            _combiner = null;
        }

        private void TryRotateItem(Quaternion rotation)
        {
            if (_inventory.Occupied == false || Quaternion.Angle(_itemHolder.localRotation, _targetRotation) > _maxRemainingAngleToRotate) return;

            _targetRotation *= rotation;
        }

        private void ProceedItemRotation()
        {
            _itemHolder.localRotation = Quaternion.RotateTowards(_itemHolder.localRotation, _targetRotation, _itemRotationSpeed * Time.deltaTime);
        }

        private void ResetItemHolder()
        {
            _itemHolder.localPosition = _itemHolderInitialPosition;
            _itemHolder.localRotation = _itemHolderInitialRotation;
            _targetRotation = _itemHolderInitialRotation;
        }
    }
}
