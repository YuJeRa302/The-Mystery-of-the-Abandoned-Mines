using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class AbilityView : MonoBehaviour
    {
        [SerializeField] protected Image _reloadingImage;
        [SerializeField] protected Image _abilityIcon;

        protected float AbilityCooldown;

        public void Initialize(Sprite sprite, float currentAbilityCooldown)
        {
            _abilityIcon.sprite = sprite;
            AbilityCooldown = currentAbilityCooldown;
        }

        public virtual void Upgrade(float currentAbilityCooldown)
        {
            AbilityCooldown = currentAbilityCooldown;
        }

        public virtual void ResetCooldownValue(float currentAbilityCooldown)
        {
            _reloadingImage.fillAmount = currentAbilityCooldown;
        }

        public virtual void ChangeCooldownValue(float currentAbilityCooldown)
        {
            _reloadingImage.fillAmount = currentAbilityCooldown / AbilityCooldown;
        }

        public virtual void ViewDestroy()
        {
            Destroy(gameObject);
        }
    }
}