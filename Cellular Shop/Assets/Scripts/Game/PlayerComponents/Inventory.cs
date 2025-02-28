using System;
using UnityEngine;
using Game.Environment;

namespace Game.PlayerComponents
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private Cell[] _cells;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;

        private int _index;

        public bool Occupied => CurrentCell.Occupied;

        private Cell CurrentCell => _cells[_index];

        private void OnValidate()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                var color = i == _index ? _selectedColor : _unselectedColor;
                _cells[i].Select(color);
            }
        }

        public void ChooseItem(int index)
        {
            if (index < 0 ||  index >= _cells.Length) throw new ArgumentOutOfRangeException("Trying to choose unexisting cell");

            CurrentCell.Deselect(_unselectedColor);
            _index = index;
            CurrentCell.Select(_selectedColor);
        }

        public void AddItem(Item item, Transform parent, float zoomTime)
        {
            if (Occupied) throw new InvalidOperationException("Trying to add an item to an occupied cell");
            if (item == null) throw new ArgumentNullException("Item cannot be null");
            if (zoomTime < 0f) throw new ArgumentOutOfRangeException("Zoom time cannot be negative");

            _cells[_index].SetItem(item, parent, zoomTime);
        }

        public bool TryProcessCombining(Combiner combiner, Action onBegan, Action onFinished, Action onStoppedCombining, float deltaTime)
        {
            return CurrentCell.TryProcessCombining(combiner, onBegan, onFinished, onStoppedCombining, ExtractItem, deltaTime);
        }

        public void StopCombining(Combiner combiner, Action onStoppedCombining)
        {
            CurrentCell.StopCombining(combiner, onStoppedCombining, ExtractItem);
        }

        public void ThrowItem(Vector3 force)
        {
            if (Occupied == false) throw new InvalidOperationException("Trying to throw an unexisting item");

            CurrentCell.ThrowItem(force);
        }

        private void ExtractItem()
        {
            CurrentCell.ExtractItem();
        }
    }
}