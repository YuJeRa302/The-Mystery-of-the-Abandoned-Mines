using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAbilityCaster : IDisposable
    {
        private readonly PlayerClassData _playerClassData;
        private readonly PersistentDataService _persistentDataService;
        private readonly int _shiftIndex = 1;

        private Player _player;
        private List<Ability> _abilities = new();
        private List<Ability> _classAbilities = new();
        private List<Ability> _legendaryAbilities = new();
        private List<PassiveAbilityView> _passiveAbilityViews = new();
        private AttributeData _abilityAttributeData;

        private AbilityPresenterFactory _abilityPresenterFactory;
        private AudioPlayer _audioPlayer;
        private AbilityFactory _abilityFactory;

        private int _abilityDuration = 0;
        private int _abilityDamage = 0;
        private int _abilityCooldownReduction = 0;
        private int _currentAbilityLevel;

        public event Action<ClassAbilityData, int> ClassAbilityTaked;
        public event Action<PassiveAttributeData> PassiveAbilityTaked;
        public event Action<ActiveAbilityData, int> AbilityTaked;
        public event Action<ActiveAbilityData> LegendaryAbilityTaked;
        public event Action<Ability> AbilityRemoved;
        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;

        public PlayerAbilityCaster(
            AbilityFactory abilityFactory,
            AbilityPresenterFactory abilityPresenterFactory,
            Player player,
            PersistentDataService persistentDataService,
            PlayerClassData playerClassData,
            AudioPlayer audioPlayer)
        {
            _abilityFactory = abilityFactory;
            _abilityPresenterFactory = abilityPresenterFactory;
            _player = player;
            _persistentDataService = persistentDataService;
            _playerClassData = playerClassData;
            _audioPlayer = audioPlayer;
        }

        public void Initialize()
        {
            foreach (ClassAbilityData ability in _playerClassData.ClassAbilityDatas)
            {
                CreateClassAbility(ability);
            }
        }

        public void TakeAbility(CardView cardView)
        {
            _activeAbilityData = null;

            if ((cardView.CardData.AttributeData as LegendaryAbilityData) != null)
            {
                if (TrySetLegendaryAbility((cardView.CardData.AttributeData as LegendaryAbilityData).UpgradeType, out Ability legendAbility))
                    CreateLegendaryAbility(legendAbility, legendAbility.AbilityAttribute);
            }

            if (cardView.CardData.AttributeData == null)
                return;

            if ((cardView.CardData.AttributeData as PassiveAttributeData) != null)
            {
                PassiveAbilityTaked?.Invoke((cardView.CardData.AttributeData as PassiveAttributeData));
            }
            else
            {
                _activeAbilityData = cardView.CardData.AttributeData as ActiveAbilityData;
                _currentAbilityLevel = cardView.CardState.CurrentLevel;

                if (TryGetAbility(_activeAbilityData as ActiveAbilityData, out Ability ability))
                    ability.Upgrade(ability.AbilityAttribute, _currentAbilityLevel, _abilityDuration, _abilityDamage, _abilityCooldownReduction);
                else
                    AbilityTaked?.Invoke(_activeAbilityData as ActiveAbilityData, cardView.CardState.CurrentLevel);
            }
        }

        public void Dispose()
        {
            DestroyAbilities();
            GC.SuppressFinalize(this);
        }

        public void AbilityDurationChanged(int value)
        {
            _abilityDuration = value;
        }

        public void AbilityDamageChanged(int value)
        {
            _abilityDamage = value;
        }

        public void AbilityCooldownReductionChanged(int value)
        {
            _abilityCooldownReduction = value;
        }

        public void CreateClassAbilityView(ClassAbilityData classAbilityData, ClassSkillButtonView classSkillButtonView, int currentLevel)
        {
            Ability newAbility;
            newAbility = _abilityFactory.CreateClassSkill(classAbilityData, false, currentLevel - 1);

            switch (classAbilityData.AbilityType)
            {
                case TypeAbility.Summon:
                    _abilityPresenterFactory.CreateSummonAbilityPresenter(
                        newAbility,
                        classSkillButtonView,
                        _player.ShotPoint,
                        _player,
                        (classAbilityData as SummonAbilityData).Summon.Summon,
                        _player.Pool);
                    break;
                case TypeAbility.ThrowAxe:
                    _abilityPresenterFactory.CreateThrowAxePresenter(
                        newAbility,
                        classSkillButtonView,
                        _player,
                        (classAbilityData as ThrowAxeClassAbility).AxemMssile);
                    break;
                case TypeAbility.JerkFront:
                    _abilityPresenterFactory.CreateJerkFrontAbilityPresenter(
                        newAbility,
                        classSkillButtonView,
                        _player,
                        (classAbilityData as JerkFrontAbilityData).PoolParticle);
                    break;
                case TypeAbility.Rage:
                    _abilityPresenterFactory.CreateRageAbilityPresenter(
                        newAbility,
                        classSkillButtonView,
                        _player,
                        (classAbilityData as RageClassAbilityData).RageEffect);
                    break;
                case TypeAbility.Epiphany:
                    _abilityPresenterFactory.CreateEpiphanyAbilityPresenter(
                        newAbility,
                        classSkillButtonView,
                        _player,
                        (classAbilityData as EpiphanyClassAbilityData).EpiphanyParticle,
                        (classAbilityData as EpiphanyClassAbilityData).Spell);
                    break;
                case TypeAbility.ShieldUp:
                    _abilityPresenterFactory.CreateShieldUpAbility(
                        newAbility,
                        classSkillButtonView,
                        _player,
                        (classAbilityData as ShieldUpAbility).PoolParticle);
                    break;
                case TypeAbility.SoulExplosion:
                    _abilityPresenterFactory.CreateSoulExplosionAbilityPresenter(
                        newAbility,
                        classSkillButtonView,
                        _player,
                        (classAbilityData as SoulExplosionAbilityData).DamageParticle,
                        (classAbilityData as SoulExplosionAbilityData).Spell);
                    break;
                case TypeAbility.DarkPact:
                    _abilityPresenterFactory.CreateDarkPactAbilityPresenter(
                        newAbility,
                        classSkillButtonView,
                        _player, (classAbilityData as DarkPactAbilityData).PoolParticle);
                    break;
                case TypeAbility.StunningBlow:
                    _abilityPresenterFactory.CreateStunningBlowAbilityPresenter(
                        newAbility,
                        classSkillButtonView,
                        _player,
                        (classAbilityData as StunningBlowClassAbilityData).PoolParticle);
                    break;
            }

            newAbility.AbilityUsed += OnAbilityUsed;
            newAbility.AbilityEnded += OnAbilityEnded;
            _classAbilities.Add(newAbility);
        }

        public void CreateAbilityView(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint)
        {
            Ability newAbility = _abilityFactory.CreateAbility(_activeAbilityData as ActiveAbilityData,
                _currentAbilityLevel,
                _abilityCooldownReduction,
                _abilityDuration,
                _abilityDamage,
                true);

            if ((_activeAbilityData as ActiveAbilityData).TypeAbility != TypeAbility.AttackAbility)
            {
                _abilityPresenterFactory.CreateAmplifierAbilityPresenter(newAbility, abilityView, particleSystem);
            }
            else
                _abilityPresenterFactory.CreateAttackAbilityPresenter(
                    newAbility,
                    abilityView,
                    _player,
                    throwPoint,
                    particleSystem,
                    (_activeAbilityData as AttackAbilityData).Spell);


            newAbility.AbilityUsed += OnAbilityUsed;
            newAbility.AbilityEnded += OnAbilityEnded;
            _abilities.Add(newAbility);
        }

        public void CreateLegendaryAbilityView(
            AbilityView abilityView,
            ParticleSystem particleSystem,
            Transform throwPoint,
            ActiveAbilityData abilityAttributeData)
        {
            Debug.Log(abilityAttributeData == null);
            Ability newAbility = _abilityFactory.CreateLegendaryAbility(
                (abilityAttributeData as LegendaryAbilityData),
                abilityAttributeData,
                _abilityCooldownReduction,
                _abilityDuration,
                _abilityDamage,
                true);

            switch (abilityAttributeData.UpgradeType)
            {
                case TypeUpgradeAbility.ElectricSphere:
                    _abilityPresenterFactory.CreateGlobularLightningPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem, (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.ElectricTrap:
                    ElectricGuardPresenter electricGuardPresenter = _abilityPresenterFactory.CreateElectricGuardPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem,
                        (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.LightningBolt:
                    ThunderPresenter thunderPresenter = _abilityPresenterFactory.CreateThunderPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem,
                        (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.Meteor:
                    _abilityPresenterFactory.CreateMetiorSowerPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        abilityAttributeData.Particle,
                        (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.FireBall:
                    _abilityPresenterFactory.CreateFirestormPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem, (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.FireCircle:
                    _abilityPresenterFactory.CreateDragonTracePresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem,
                        (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.ShowBall:
                    _abilityPresenterFactory.CreateSnowfallPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem,
                        (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.IceBolt:
                    _abilityPresenterFactory.CreateIciAvalanchePresenter(
                        newAbility,
                        abilityView,
                        _player,
                        abilityAttributeData.Particle,
                        (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
                case TypeUpgradeAbility.FrostNova:
                    _abilityPresenterFactory.CreateBuranPresenter(
                        newAbility,
                         abilityView,
                        _player,
                        abilityAttributeData.Particle,
                        (abilityAttributeData as LegendaryAbilityData).LegendariSpell);
                    break;
            }

            newAbility.AbilityUsed += OnAbilityUsed;
            newAbility.AbilityEnded += OnAbilityEnded;
            _legendaryAbilities.Add(newAbility);
        }

        public void CreatePassiveAbilityView(PassiveAbilityView passiveAbilityView)
        {
            _passiveAbilityViews.Add(passiveAbilityView);
        }

        private void CreateClassAbility(ClassAbilityData abilityData)
        {
            ClassAbilityState classAbilityState = _persistentDataService.PlayerProgress.ClassAbilityService.GetClassAbilityStateById(abilityData.Id);
            ClassAbilityTaked?.Invoke(abilityData, classAbilityState.CurrentLevel);
        }

        private void OnAbilityUsed(Ability ability)
        {
            AbilityUsed?.Invoke(ability);
            _audioPlayer.PlayCharesterAudio(ability.AudioClip);
        }

        private void OnAbilityEnded(Ability ability)
        {
            AbilityEnded?.Invoke(ability);
        }

        private void DestroyAbilities()
        {
            foreach (Ability ability in _abilities)
            {
                ability.AbilityUsed -= OnAbilityUsed;
                ability.AbilityEnded -= OnAbilityEnded;
                ability.Dispose();
            }

            foreach (var ability in _classAbilities)
            {
                ability.AbilityUsed -= OnAbilityUsed;
                ability.AbilityEnded -= OnAbilityEnded;
                ability.Dispose();
            }
        }

        private bool TrySetLegendaryAbility(TypeUpgradeAbility typeUpgrate, out Ability newability)
        {
            bool isFind = false;
            newability = null;

            if (_passiveAbilityViews.Count > 0)
            {
                if (_abilities.Count > 0)
                {
                    foreach (Ability ability in _abilities)
                    {
                        foreach (PassiveAbilityView passiveAbilityView in _passiveAbilityViews)
                        {
                            if (ability.TypeMagic == passiveAbilityView.TypeMagic)
                            {
                                if (ability.CurrentLevel == ability.MaxLevel - _shiftIndex)
                                {
                                    if (ability.TypeUpgradeMagic == typeUpgrate)
                                    {
                                        newability = ability;
                                        isFind = true;
                                        return isFind;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return isFind;
        }

        private bool TryGetAbility(ActiveAbilityData abilityAttributeData, out Ability oldAbility)
        {
            bool isFind = false;
            oldAbility = null;

            if (_abilities.Count > 0)
            {
                foreach (Ability ability in _abilities)
                {
                    if (ability.TypeAbility == abilityAttributeData.TypeAbility)
                    {
                        if (ability.TypeUpgradeMagic == abilityAttributeData.UpgradeType)
                        {
                            if ((abilityAttributeData as AttackAbilityData) != null)
                            {
                                if (ability.TypeAttackAbility == (abilityAttributeData as AttackAbilityData).TypeAttackAbility)
                                {
                                    oldAbility = ability;
                                    isFind = true;
                                }
                            }
                            else
                            {
                                oldAbility = ability;
                                isFind = true;
                            }
                        }
                    }
                }
            }

            return isFind;
        }

        private void CreateLegendaryAbility(Ability ability, ActiveAbilityData abilityAttributeData)
        {
            foreach (Ability existingAbility in _legendaryAbilities)
            {
                if (existingAbility.TypeUpgradeMagic == ability.TypeUpgradeMagic)
                    return;
            }

            ability.Dispose();

            LegendaryAbilityTaked?.Invoke((abilityAttributeData as AttackAbilityData).LegendaryAbilityData);
        }
    }
}