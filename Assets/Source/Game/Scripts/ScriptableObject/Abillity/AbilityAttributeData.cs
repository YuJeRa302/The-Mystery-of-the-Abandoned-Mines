using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Ability Attribute", menuName = "Create Attribute/Ability Attribute", order = 51)]
    public abstract class AbilityAttributeData : AttributeData
    {
        [SerializeField] private TypeAbility _abilityType;
        [SerializeField] private AbilityView _abilityView;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private TypeMagic _typeMagic;
        [SerializeField] private LegendaryAbilityData _legendaryAbilityData;

        public LegendaryAbilityData LegendaryAbilityData => _legendaryAbilityData;
        public AbilityView AbilityView => _abilityView;
        public TypeAbility TypeAbility => _abilityType;
        public TypeMagic TypeMagic => _typeMagic;
        public ParticleSystem ParticleSystem => _particleSystem;
        public AudioClip AudioClip => _audioClip;
    }
}