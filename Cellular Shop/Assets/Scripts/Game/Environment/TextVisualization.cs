using UnityEngine;
using TMPro;

namespace Game.Environment
{
    public class TextVisualization : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _beginning;

        public void Visualize(string text)
        {
            _text.text = _beginning + text;
        }
    }
}