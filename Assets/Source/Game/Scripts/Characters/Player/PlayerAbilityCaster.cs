using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAbilityCaster : IDisposable
    {
        private readonly int _shiftIndex = 1;

        private Player _player;
        private PlayerView _playerView;
        private TemporaryData _temporaryData;
        private List<Ability> _abilities = new ();
        private List<Ability> _classAbilities = new ();
        private List<Ability> _legendaryAbilities = new ();
        private List<ClassAbilityData> _classAbilityDatas = new ();
        private List<PassiveAbilityView> _passiveAbilityViews = new ();
        private AbilityAttributeData _abilityAttributeData;
        private AbilityPresenterFactory _abilityPresenterFactory;
        private AbilityFactory _abilityFactory;
        private int _abilityDuration = 0;
        private int _abilityDamage = 0;
        private int _abilityCooldownReduction = 0;
        private int _currentAbilityLevel;

        public event Action<ClassAbilityData, int> ClassSkillInitialized;
        public event Action<PassiveAttributeData> PassiveAbilityTaked;
        public event Action<AbilityAttributeData, int> AbilityTaked;
        public event Action<AbilityAttributeData> LegendaryAbilityTaked;
        public event Action<Ability> AbilityRemoved;
        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;

        public List<ClassAbilityData> ClassAbilityDatas => _classAbilityDatas;

        public PlayerAbilityCaster(AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenterFactory, Player player, PlayerView playerView, TemporaryData temporaryData)
        {
            _abilityFactory = abilityFactory;
            _abilityPresenterFactory = abilityPresenterFactory;
            _player = player;
            _playerView = playerView;
            _temporaryData = temporaryData;
            _playerView.AbilityViewCreated += OnAbilityViewCreated;
            _playerView.CreatedClassSkillView += OnClassSkillsCreated;
            _playerView.LegendaryAbilityViewCreated += OnLegendaryAbilityViewCreated;
            _playerView.PassiveAbilityViewCreated += OnPassiveAbilityViewCreated;
        }

        public void Initialize()
        {
            foreach (var skill in _temporaryData.PlayerClassData.ClassAbilityDatas)
            {
                CreateClassSkill(skill);
            }
        }

        public void TakeAbility(CardView cardView)
        {
            _abilityAttributeData = null;

            if (cardView.CardData.LegendaryAbilityData != null)
            {
                if (TrySetLegendaryAbility(cardView.CardData.LegendaryAbilityData.TypeUpgradeMagic, out Ability legendAbility))
                    CreateLegendaryAbility(legendAbility, legendAbility.AbilityAttribute);
            }
            
            if (cardView.CardData.AttributeData == null)
                return;

            if ((cardView.CardData.AttributeData as PassiveAttributeData) != null)
            {
                if ((cardView.CardData.AttributeData as PassiveAttributeData).TypeAbility == TypeAbility.PassiveAbility)
                {
                    PassiveAbilityTaked?.Invoke((cardView.CardData.AttributeData as PassiveAttributeData));
                }
            }
            else
            {
                _abilityAttributeData = cardView.CardData.AttributeData as AbilityAttributeData;
                _currentAbilityLevel = cardView.CardState.CurrentLevel;

                if (TryGetAbility(_abilityAttributeData, out Ability ability))
                    ability.Upgrade(ability.AbilityAttribute, _currentAbilityLevel);
                else
                    AbilityTaked?.Invoke(_abilityAttributeData, cardView.CardState.CurrentLevel);
            }
        }

        public void Dispose()
        {
            _playerView.AbilityViewCreated -= OnAbilityViewCreated;
            _playerView.PassiveAbilityViewCreated -= OnPassiveAbilityViewCreated;
            _playerView.LegendaryAbilityViewCreated -= OnLegendaryAbilityViewCreated;
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

        private void OnPassiveAbilityViewCreated(PassiveAbilityView passiveAbilityView)
        {
            _passiveAbilityViews.Add(passiveAbilityView);
        }

        private void CreateClassSkill(ClassAbilityData abilityData)
        {
            ClassAbilityState classAbilityState = _temporaryData.GetClassAbilityState(abilityData.Id);
            ClassSkillInitialized?.Invoke(abilityData, classAbilityState.CurrentLevel);
        }

        private void OnClassSkillsCreated(ClassAbilityData classAbilityData, ClassSkillButtonView classSkillButtonView, int currentLvl)
        {
            Ability newAbility;
            newAbility = _abilityFactory.CreateClassSkill(classAbilityData, false, currentLvl);

            if (classAbilityData.AbilityType == TypeAbility.Summon)
                _abilityPresenterFactory.CreateSummonAbilityPresenter(newAbility, classSkillButtonView, _player.ShotPoint, _player, (classAbilityData as SummonAbilityData).Summon.Summon, _player.Pool);

            if (classAbilityData.AbilityType == TypeAbility.ThrowAxe)
                _abilityPresenterFactory.CreateThrowAxePresenter(newAbility, classSkillButtonView, _player, (classAbilityData as ThrowAxeClassAbility).AxemMssile);

            if (classAbilityData.AbilityType == TypeAbility.JerkFront)
                _abilityPresenterFactory.CreateJerkFrontAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as JerkFrontAbilityData).PoolParticle);

            if (classAbilityData.AbilityType == TypeAbility.Rage)
            {
                _abilityPresenterFactory.CreateRageAbilityPresenter(newAbility,
                    classSkillButtonView, _player, (classAbilityData as RageClassAbilityData).RageEffect);
            }

            if (classAbilityData.AbilityType == TypeAbility.Epiphany)
                _abilityPresenterFactory.CreateEpiphanyAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as EpiphanyClassAbilityData).EpiphanyParticle);

            if (classAbilityData.AbilityType == TypeAbility.ShieldUp)
                _abilityPresenterFactory.CreateShieldUpAbility(newAbility, classSkillButtonView, _player, (classAbilityData as ShieldUpAbility).PoolParticle);

            if (classAbilityData.AbilityType == TypeAbility.SoulExplosion)
                _abilityPresenterFactory.CreateSoulExplosionAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as SoulExplosionAbilityData).DamageParticle);

            if (classAbilityData.AbilityType == TypeAbility.DarkPact)
                _abilityPresenterFactory.CreateDarkPactAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as DarkPactAbilityData).PoolParticle);

            if (classAbilityData.AbilityType == TypeAbility.StunningBlow)
                _abilityPresenterFactory.CreateStunningBlowAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as StunningBlowClassAbilityData).PoolParticle);

            newAbility.AbilityUsed += OnAbilityUsed;
            newAbility.AbilityEnded += OnAbilityEnded;
            _classAbilities.Add(newAbility);
        }

        private void OnAbilityViewCreated(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint)
        {
            Ability newAbility = _abilityFactory.CreateAbility(_abilityAttributeData, _currentAbilityLevel, _abilityCooldownReduction, _abilityDuration, _abilityDamage, true);

            if (_abilityAttributeData.TypeAbility != TypeAbility.AttackAbility)
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
                    (_abilityAttributeData as AttackAbilityData).Spell);


            newAbility.AbilityUsed += OnAbilityUsed;
            newAbility.AbilityEnded += OnAbilityEnded;
            _abilities.Add(newAbility);
        }

        private void OnLegendaryAbilityViewCreated(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint, AbilityAttributeData abilityAttributeData)
        {
            Ability newAbility = _abilityFactory.CreateLegendaryAbility(
                (abilityAttributeData as AttackAbilityData).LegendaryAbilityData,
                abilityAttributeData,
                _abilityCooldownReduction,
                _abilityDuration,
                _abilityDamage, 
                true);

            switch (abilityAttributeData.TypeUpgradeMagic)
            {
                case TypeUpgradeAbility.ElectricSphere:
                    _abilityPresenterFactory.CreateGlobularLightningPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem, (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.ElectricTrap:
                    ElectricGuardPresenter electricGuardPresenter = _abilityPresenterFactory.CreateElectricGuardPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.LightningBolt:
                    ThunderPresenter thunderPresenter = _abilityPresenterFactory.CreateThunderPresenter(newAbility,
                        abilityView,
                        _player,
                        particleSystem,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.Meteor:
                    _abilityPresenterFactory.CreateMetiorSowerPresenter(newAbility,
                        abilityView,
                        _player,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.Particle, 
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.FireBall:
                    _abilityPresenterFactory.CreateFirestormPresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem, (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.FireCircle:
                    _abilityPresenterFactory.CreateDragonTracePresenter(
                        newAbility,
                        abilityView,
                        _player,
                        particleSystem,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.ShowBall:
                    _abilityPresenterFactory.CreateSnowfallPresenter(
                        newAbility, abilityView, _player, particleSystem,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.IceBolt:
                    _abilityPresenterFactory.CreateIciAvalanchePresenter(newAbility,
                        abilityView,
                        _player,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.Particle,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
                case TypeUpgradeAbility.FrostNova:
                    _abilityPresenterFactory.CreateBuranPresenter(newAbility,
                         abilityView,
                        _player,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.Particle,
                        (abilityAttributeData as AttackAbilityData).LegendaryAbilityData.LegendaryAbilitySpell);
                    break;
            }

            newAbility.AbilityUsed += OnAbilityUsed;
            newAbility.AbilityEnded += OnAbilityEnded;
            _legendaryAbilities.Add(newAbility);
        }

        private void OnAbilityUsed(Ability ability)
        {
            AbilityUsed?.Invoke(ability);
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
                            if (ability.TypeMagic == passiveAbilityView.TypeMagic)///
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

        private bool TryGetAbility(AbilityAttributeData abilityAttributeData, out Ability oldAbility)
        {
            bool isFind = false;
            oldAbility = null;

            if (_abilities.Count > 0)
            {
                foreach (Ability ability in _abilities)
                {
                    if (ability.TypeAbility == abilityAttributeData.TypeAbility)
                    {
                        if(ability.TypeUpgradeMagic == abilityAttributeData.TypeUpgradeMagic)
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

        private void CreateLegendaryAbility(Ability ability, AbilityAttributeData abilityAttributeData)
        {
            foreach (var item in _legendaryAbilities)
            {
                if (item.TypeUpgradeMagic == ability.TypeUpgradeMagic)
                {
                    return;
                }
            }

            ability.Dispose();
            LegendaryAbilityTaked?.Invoke(abilityAttributeData);
        }
    }
}