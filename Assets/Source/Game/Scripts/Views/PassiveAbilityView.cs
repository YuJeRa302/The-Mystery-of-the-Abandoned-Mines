using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class PassiveAbilityView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public TypeMagic TypeMagic { get; private set; }

        public void Initialize(PassiveAttributeData passiveAttributeData)
        {
            TypeMagic = passiveAttributeData.TypeMagic;
            _icon.sprite = passiveAttributeData.Icon;
        }
    }
}