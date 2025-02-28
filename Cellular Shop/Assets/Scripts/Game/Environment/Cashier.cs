using UnityEngine;

namespace Game.Environment
{
    public class Cashier : Combiner
    {
        [SerializeField] private Transform _itemTarget;
        [SerializeField] private TextVisualization _textVisualization;
        [SerializeField] private float _breakTime;
        [SerializeField] private float _movementTime;
        [SerializeField] private float _registerTime;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Quaternion _startRotation;
        private Quaternion _endRotation;
        private int _total = 0;
        private bool _registered;
        private float _breakProgress;
        private float _progressIn;
        private float _registerProgress;
        private float _progressOut;

        public override bool ExtractItemOnFinishing => false;

        public override bool ExtractItemOnExiting => true;

        private void Update()
        {
            if (_breakProgress < 1f)
            {
                _breakProgress += Time.deltaTime / _breakTime;
            }
        }

        protected override bool TryProcessCombining(Item item, float deltaTime, out bool finished)
        {
            finished = false;
            if (_breakProgress < 1f)
            {
                return false;
            }

            if (_progressIn == 0f)
            {
                Initialize(item);
            }
            _progressIn = ProcessMovement(item, _startPosition, _endPosition, _startRotation, _endRotation, _progressIn, _movementTime, deltaTime);
            var movementIn = _progressIn >= 1f;
            var registered = false;
            if (movementIn)
            {
                _registerProgress = ProcessRegister(item, _registerProgress, _registerTime, deltaTime);
                registered = _registerProgress >= 1f;
            }
            if (registered)
            {
                if (_registered == false)
                {
                    _total += item.Cost;
                    _textVisualization.Visualize(_total.ToString());
                    _registered = true;
                }
                _progressOut = ProcessMovement(item, _endPosition, item.ParentPosition, _endRotation, item.ParentRotation, _progressOut, _movementTime, deltaTime);
                finished = _progressOut >= 1f;
                if (finished)
                {
                    ResetState();
                }
            }
            return true;
        }

        protected override void Exit()
        {
            ResetState();
        }

        private float ProcessMovement(Item item, Vector3 startPosition, Vector3 endPosition,
            Quaternion startRotation, Quaternion endRotation, float progress, float movementTime, float deltaTime)
        {
            progress += deltaTime / movementTime;
            var position = Vector3.Lerp(startPosition, endPosition, progress);
            var rotation = Quaternion.Lerp(startRotation, endRotation, progress);
            item.ProcessCombine(_itemTarget, position, rotation);
            return progress;
        }

        private float ProcessRegister(Item item, float registerProgress, float registerTime, float deltaTime)
        {
            registerProgress += deltaTime / registerTime;
            return registerProgress;
        }

        private void Initialize(Item item)
        {
            _startPosition = item.Position;
            _startRotation = item.Rotation;
            _endPosition = _itemTarget.position;
            _endRotation = _itemTarget.rotation;
        }

        private void ResetState()
        {
            _breakProgress = 0f;
            _progressIn = 0f;
            _registerProgress = 0f;
            _progressOut = 0f;
            _registered = false;
        }
    }
}
