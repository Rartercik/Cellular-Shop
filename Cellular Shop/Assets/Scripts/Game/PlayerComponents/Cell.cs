using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Environment;

namespace Game.PlayerComponents
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _icon;

        private Item _item;

        public bool Occupied => _item != null;

        public void SetItem(Item item, Transform parent, float zoomTime)
        {
            _icon.sprite = item.Icon;
            _icon.enabled = true;
            _item = item;
            _item.Grab(parent, zoomTime);
        }

        public void Select(Color selectedColor)
        {
            _image.color = selectedColor;
            if (_item != null)
            {
                _item.SetActive(true);
            }
        }

        public void Deselect(Color unselectedColor)
        {
            _image.color = unselectedColor;
            if (_item != null)
            {
                _item.SetActive(false);
            }
        }

        public bool TryProcessCombining(Combiner combiner, Action onBegan, Action onFinished, Action onExited, Action extractItem, float deltaTime)
        {
            return combiner.TryProcessCombining(_item, onBegan, onFinished, onExited, extractItem, deltaTime);
        }

        public void StopCombining(Combiner combiner, Action onStoppedCombining, Action extractItem)
        {
            combiner.Exit(_item, onStoppedCombining, extractItem);
        }

        public void ExtractItem()
        {
            _item.SetFree();
            FreeCell();
        }

        public void ThrowItem(Vector3 force)
        {
            _item.Throw(force);
            FreeCell();
        }

        private void FreeCell()
        {
            _icon.sprite = null;
            _icon.enabled = false;
            _item = null;
        }
    }
}