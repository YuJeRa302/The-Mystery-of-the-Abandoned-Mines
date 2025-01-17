using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Default Ability", menuName = "Create Default Ability", order = 51)]
    public class DefaultAbilityData : AbilityAttributeData
    {
        [SerializeField] private TypeEffect _typeEffect;

        public TypeEffect TypeEffect => _typeEffect;
    }
}