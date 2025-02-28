using System;
using UnityEngine;

namespace Game.Environment
{
    [RequireComponent(typeof(Collider))]
    public abstract class Combiner : MonoBehaviour
    {
        public abstract bool ExtractItemOnFinishing { get; }

        public abstract bool ExtractItemOnExiting { get; }

        private bool _began;

        public bool TryProcessCombining(Item item, Action onBegan, Action onFinished, Action onExited, Action extractItem, float deltaTime)
        {
            var success = TryProcessCombining(item, deltaTime, out var finished);
            if (_began == false && success)
            {
                _began = true;
                onBegan?.Invoke();
            }
            if (finished)
            {
                _began = false;
                if (ExtractItemOnFinishing)
                {
                    extractItem.Invoke();
                }
                else
                {
                    item.StopCombining(ExtractItemOnFinishing);
                }
                onFinished?.Invoke();
                return true;
            }

            if (_began && success == false)
            {
                Exit(item, onExited, extractItem);
                return false;
            }
            _began = success;
            return success;
        }

        public void Exit(Item item, Action onExited, Action extractItem)
        {
            if (_began == false) throw new InvalidOperationException("Can't exit combining without begining to combine");

            _began = false;
            Exit();
            if (ExtractItemOnExiting)
            {
                extractItem.Invoke();
            }
            else
            {
                item.StopCombining(false);
            }
            onExited?.Invoke();
        }

        protected abstract bool TryProcessCombining(Item item, float deltaTime, out bool finished);
        protected abstract void Exit();
    }
}