using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAbilityCaster : IDisposable
    {
        private Player _player;
        private PlayerView _playerView;

        private List<Ability> _abilities = new ();
        private List<Ability> _classAbilities = new ();
        private AbilityAttributeData _abilityAttributeData;
        private AbilityPresenterFactory _abilityPresenterFactory;
        private AbilityFactory _abilityFactory;
        private List<ClassAbilityData> _classAbilityDatas = new ();
        private TemporaryData _temporaryData;

        private int _abilityDuration = 0;
        private int _abilityDamage = 0;
        private int _abilityCooldownReduction = 0;
        private int _currentAbilityLevel;

        public List<ClassAbilityData> ClassAbilityDatas => _classAbilityDatas;

        public event Action<AbilityAttributeData, int> AbilityTaked;
        public event Action<ClassAbilityData, int> ClassSkillInitialized;
        public event Action<Ability> AbilityRemoved;
        public event Action<Ability> AbilityUsed;//++
        public event Action<Ability> AbilityEnded;//++
        public event Action<Ability> ClassAbilityUsed;
        public event Action<Ability> ClassAbilityEnded;

        public PlayerAbilityCaster(AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenterFactory, 
            Player player, PlayerView playerView, TemporaryData temporaryData) 
        {
            _abilityFactory = abilityFactory;
            _abilityPresenterFactory = abilityPresenterFactory;
            _player = player;
            _playerView = playerView;
            _temporaryData = temporaryData;

            _playerView.AbilityViewCreated += OnAbilityViewCreated;
            _playerView.CreatedClassSkillView += OnClassSkillsCreated;
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
            if (cardView.CardData.AttributeData == null)
                return;

            _abilityAttributeData = cardView.CardData.AttributeData as AbilityAttributeData;
            _currentAbilityLevel = cardView.CardState.CurrentLevel;

            if (TryGetAbility(_abilityAttributeData, out Ability ability))
                ability.Upgrade(_abilityAttributeData, _currentAbilityLevel);
            else
                AbilityTaked?.Invoke(_abilityAttributeData, cardView.CardState.CurrentLevel);
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
            {
                _abilityPresenterFactory.CreateSummonAbilityPresenter(newAbility, classSkillButtonView, _player.ShotPoint, _player, (classAbilityData as SummonAbilityData).Summon.Summon, _player.Pool);
            }

            if (classAbilityData.AbilityType == TypeAbility.ThrowAxe)
            {
                _abilityPresenterFactory.CreateThrowAxePresenter(newAbility, classSkillButtonView, _player, (classAbilityData as ThrowAxeClassAbility).AxemMssile);
            }

            if (classAbilityData.AbilityType == TypeAbility.JerkFront)
            {
                _abilityPresenterFactory.CreateJerkFrontAnillityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as JerkFrontAbilityData).PoolParticle);
            }

            if (classAbilityData.AbilityType == TypeAbility.Rage)
            {
                int boostDamage = 0;
                float boosMoveSpeed = 0;
                int boosArmor = 0;

                foreach (CardParameter parameter in classAbilityData.Parameters[currentLvl].CardParameters)
                {
                    if (parameter.TypeParameter == TypeParameter.Damage)
                        boostDamage = parameter.Value;

                    if (parameter.TypeParameter == TypeParameter.MoveSpeed)
                        boosMoveSpeed = parameter.Value;

                    if (parameter.TypeParameter == TypeParameter.Armor)
                        boosArmor = parameter.Value;
                }

                _abilityPresenterFactory.CreateRageAbilityPresenter(newAbility,
                    classSkillButtonView, _player, boostDamage, boosMoveSpeed, boosArmor, (classAbilityData as RageClassAbilityData).RageEffect);
            }

            if (classAbilityData.AbilityType == TypeAbility.Epiphany)
            {
                _abilityPresenterFactory.CreateEpiphanyAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as EpiphanyClassAbilityData).EpiphanyParticle);
            }

            if (classAbilityData.AbilityType == TypeAbility.ShieldUp)
            {
                _abilityPresenterFactory.CreateShieldUpAbility(newAbility, classSkillButtonView, _player, (classAbilityData as ShieldUpAbility).PoolParticle);
            }

            if (classAbilityData.AbilityType == TypeAbility.SoulExplosion)
            {
                _abilityPresenterFactory.CreateSoulExplosionAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as SoulExplosionAbilityData).DamageParticle);
            }

            if (classAbilityData.AbilityType == TypeAbility.DarkPact)
            {
                _abilityPresenterFactory.CreateDarkPactAbilityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as DarkPactAbilityData).PoolParticle);
            }

            newAbility.AbilityUsed += OnAbilityUsed;
            newAbility.AbilityEnded += OnAbilityEnded;
            _classAbilities.Add(newAbility);
        }

        private void OnAbilityViewCreated(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint)
        {
            Ability newAbility = _abilityFactory.Create(_abilityAttributeData, _currentAbilityLevel, _abilityCooldownReduction, _abilityDuration, _abilityDamage, true);


            if (_abilityAttributeData.AbilityType != TypeAbility.AttackAbility)
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
            }

            foreach (Ability ability in _abilities)
                ability.Dispose();

            foreach (var ability in _classAbilities)
            {
                ability.AbilityUsed -= OnAbilityUsed;
                ability.AbilityEnded -= OnAbilityEnded;
                ability.Dispose();
            }
        }

        private bool TryGetAbility(AbilityAttributeData abilityAttributeData, out Ability oldAbility)
        {
            bool isFind = false;
            oldAbility = null;

            if (_abilities.Count > 0)
            {
                foreach (Ability ability in _abilities)
                {
                    //foreach (var type in ability.TypeAbility)
                    //{
                        
                    //    //if (type == abilityAttributeData.AbilityTypes)
                    //    //{
                    //    //    if ((abilityAttributeData as AttackAbilityData) != null)
                    //    //    {
                    //    //        if (ability.TypeAttackAbility == (abilityAttributeData as AttackAbilityData).TypeAttackAbility)
                    //    //        {
                    //    //            oldAbility = ability;
                    //    //            isFind = true;
                    //    //        }
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        oldAbility = ability;
                    //    //        isFind = true;
                    //    //    }
                    //    //}
                    //}
                }
            }

            return isFind;
        }

        public void Dispose()
        {
            DestroyAbilities();
            _playerView.AbilityViewCreated -= OnAbilityViewCreated;
            GC.SuppressFinalize(this);
        }
    }
}