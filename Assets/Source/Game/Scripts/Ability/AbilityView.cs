using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class AbilityView : MonoBehaviour
    {
        [SerializeField] protected Image _reloadingImage;
        [SerializeField] protected Image _abilityIcon;

        protected float _defaultDelay;

        public void Initialize(Sprite sprite, float delay)
        {
            _abilityIcon.sprite = sprite;
            _defaultDelay = delay;
        }

        public virtual void Upgrade(float delay)
        {
            _defaultDelay = delay;
        }

        public virtual void ResetCooldownValue(float delay)
        {
            _reloadingImage.fillAmount = delay;
        }

        public virtual void ChangeCooldownValue(float currentDelay)
        {
            _reloadingImage.fillAmount = currentDelay / _defaultDelay;
        }

        public virtual void ViewDestroy()
        {
            Destroy(gameObject);
        }
    }
}