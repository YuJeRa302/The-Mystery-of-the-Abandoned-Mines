using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Ability Attribute", menuName = "Create Ability Attribute", order = 51)]
    public abstract class AbilityAttributeData : AttributeData
    {
        [SerializeField] private TypeAbility _typeAbility;
        [SerializeField] private AbilityView _abilityView;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private AudioClip _audioClip;

        public AbilityView AbilityView => _abilityView;
        public TypeAbility TypeAbility => _typeAbility;
        public ParticleSystem ParticleSystem => _particleSystem;
        public AudioClip AudioClip => _audioClip;
    }
}