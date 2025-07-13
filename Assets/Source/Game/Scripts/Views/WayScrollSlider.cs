using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class WayScrollSlider : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Slider _slider;

        private bool _isSliderDragging = false;

        private void Start()
        {
            _slider.onValueChanged.AddListener(OnSliderChanged);
            _scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }

        private void OnSliderChanged(float value)
        {
            if (!_isSliderDragging)
            {
                _scrollRect.verticalNormalizedPosition = value;
            }
        }

        private void OnScrollChanged(Vector2 scrollPos)
        {
            _slider.SetValueWithoutNotify(_scrollRect.verticalNormalizedPosition);
        }
    }
}