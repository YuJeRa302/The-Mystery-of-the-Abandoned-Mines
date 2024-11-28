using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class AbilityView : MonoBehaviour
    {
        [SerializeField] private Image _reloadingImage;
        [SerializeField] private Image _abilityIcon;

        private float _defaultDelay;

        public void Initialize(Sprite sprite, float delay)
        {
            _abilityIcon.sprite = sprite;
            _defaultDelay = delay;
        }

        public void Upgrade(float delay)
        {
            _defaultDelay = delay;
        }

        public void ResetCooldownValue(float delay)
        {
            _reloadingImage.fillAmount = delay;
        }

        public void ChangeCooldownValue(float currentDelay)
        {
            _reloadingImage.fillAmount = currentDelay / _defaultDelay;
        }

        public void ViewDestroy()
        {
            Destroy(gameObject);
        }
    }
}