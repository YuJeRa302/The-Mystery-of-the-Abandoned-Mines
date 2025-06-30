using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Ability Attribute", menuName = "Create Attribute/Ability Attribute ", order = 51)]
    public abstract class ActiveAbilityData : AttributeData
    {
        [SerializeField] private AbilityView _abilityView;
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private DamageSource _damage;
        [SerializeField] private TypeUpgradeAbility _upgradeType;
        [SerializeField] private TypeAbility _type;
        [SerializeField] private TypeAttackAbility _typeAttack;

        public AbilityView AbilityView => _abilityView;
        public ParticleSystem Particle => _particle;
        public AudioClip AudioClip => _audioClip;
        public TypeUpgradeAbility UpgradeType => _upgradeType;
        public TypeAbility TypeAbility => _type;
        public TypeAttackAbility TypeAttackAbility => _typeAttack;
        public DamageSource Damage => _damage;
    }
}