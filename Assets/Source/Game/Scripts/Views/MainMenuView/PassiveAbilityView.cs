using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class PassiveAbilityView : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public TypeMagic TypeMagic { get; private set; }

        public void Initialize(PassiveAttributeData passiveAttributeData)
        {
            TypeMagic = passiveAttributeData.MagicType;
            _icon.sprite = passiveAttributeData.Icon;
        }
    }
}