using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AbilityView : MonoBehaviour
    {
        [SerializeField] private Image _reloadingImage;
        [SerializeField] private Image _abilityIcon;

        private float _abilityCooldown;

        public void Initialize(Sprite sprite, float currentAbilityCooldown)
        {
            _abilityIcon.sprite = sprite;
            _abilityCooldown = currentAbilityCooldown;
        }

        public void Initialize(Sprite sprite)
        {
            _abilityIcon.sprite = sprite;
        }

        public virtual void Upgrade(float currentAbilityCooldown)
        {
            _abilityCooldown = currentAbilityCooldown;
        }

        public virtual void ResetCooldownValue(float currentAbilityCooldown)
        {
            _reloadingImage.fillAmount = currentAbilityCooldown;
        }

        public virtual void ChangeCooldownValue(float currentAbilityCooldown)
        {
            _reloadingImage.fillAmount = currentAbilityCooldown / _abilityCooldown;
        }

        public virtual void ViewDestroy()
        {
            Destroy(gameObject);
        }
    }
}