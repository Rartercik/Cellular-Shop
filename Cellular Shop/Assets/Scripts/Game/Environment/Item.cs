using UnityEngine;

namespace Game.Environment
{
    [RequireComponent(typeof(Collider))]
    public class Item : MonoBehaviour
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private Transform _transform;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Vector3 _grabbedLocalRotation;
        [SerializeField] private int _cost;

        private Transform _parent;

        public Sprite Icon => _icon;
        public Vector3 Position => _transform.position;
        public Vector3 ParentPosition => _parent.position;
        public Quaternion Rotation => _transform.rotation * Quaternion.Euler(_grabbedLocalRotation);
        public Quaternion ParentRotation => _parent.rotation;
        public int Cost => _cost;

        public void Grab(Transform parent, float zoomTime)
        {
            _rigidbody.isKinematic = true;
            SetHeldPosition(parent);
        }

        public void ProcessCombine(Transform parent, Vector3 position, Quaternion rotation)
        {
            _transform.parent = parent;
            _transform.position = position;
            _transform.rotation = rotation * Quaternion.Euler(_grabbedLocalRotation);
        }

        public void StopCombining(bool setFree)
        {
            if (setFree)
            {
                SetFree();
            }
            else
            {
                SetHeldPosition(_parent);
            }
        }

        public void SetFree()
        {
            _parent = null;
            _transform.parent = null;
            _rigidbody.isKinematic = false;
        }

        public void Throw(Vector3 force)
        {
            SetFree();
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        private void SetHeldPosition(Transform parent)
        {
            _parent = parent;
            _transform.parent = parent;
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.Euler(_grabbedLocalRotation);
        }
    }
}