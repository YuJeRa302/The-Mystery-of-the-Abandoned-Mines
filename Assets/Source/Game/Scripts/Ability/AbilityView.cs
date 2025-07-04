using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class AbilityView : MonoBehaviour
    {
        [SerializeField] protected Image ReloadingImage;
        [SerializeField] protected Image AbilityIcon;

        protected float AbilityCooldown;

        public void Initialize(Sprite sprite, float currentAbilityCooldown)
        {
            AbilityIcon.sprite = sprite;
            AbilityCooldown = currentAbilityCooldown;
        }

        public void Initialize(Sprite sprite)
        {
            AbilityIcon.sprite = sprite;
        }

        public virtual void Upgrade(float currentAbilityCooldown)
        {
            AbilityCooldown = currentAbilityCooldown;
        }

        public virtual void ResetCooldownValue(float currentAbilityCooldown)
        {
            ReloadingImage.fillAmount = currentAbilityCooldown;
        }

        public virtual void ChangeCooldownValue(float currentAbilityCooldown)
        {
            ReloadingImage.fillAmount = currentAbilityCooldown / AbilityCooldown;
        }

        public virtual void ViewDestroy()
        {
            Destroy(gameObject);
        }
    }
}