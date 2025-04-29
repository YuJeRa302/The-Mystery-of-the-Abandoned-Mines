using UnityEngine;
using UnityEngine.UI;

public class WayScrollSlider : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Slider _slider;
    
    private bool isSliderDragging = false;

    private void Start()
    {
        _slider.onValueChanged.AddListener(OnSliderChanged);
        _scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void OnSliderChanged(float value)
    {
        if (!isSliderDragging)
        {
            _scrollRect.verticalNormalizedPosition = value;
        }
    }

    private void OnScrollChanged(Vector2 scrollPos)
    {
        _slider.SetValueWithoutNotify(_scrollRect.verticalNormalizedPosition);
    }
}