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
        private AbilityAttributeData _abilityAttributeData;
        private AbilityPresenterFactory _abilityPresenterFactory;
        private AbilityFactory _abilityFactory;
        private List<ClassAbilityData> _classAbilityDatas = new();

        private int _abilityDuration = 0;
        private int _abilityDamage = 0;
        private int _abilityCooldownReduction = 0;
        private int _currentAbilityLevel;

        public List<ClassAbilityData> ClassAbilityDatas => _classAbilityDatas;

        public event Action<AbilityAttributeData, int> AbilityTaked;
        public event Action<Ability> AbilityRemoved;
        public event Action<Ability> AbilityUsed;//++
        public event Action<Ability> AbilityEnded;//++

        private void OnDestroy()
        {
            _playerView.AbilityViewCreated -= OnAbilityViewCreated;
            DestroyAbilities();
        }

        public PlayerAbilityCaster(AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenterFactory, Player player, PlayerView playerView) 
        {
            _abilityFactory = abilityFactory;
            _abilityPresenterFactory = abilityPresenterFactory;
            _player = player;
            _playerView = playerView;

            foreach (var skill in _player.ClassAbilityDatas)
            {
                _classAbilityDatas.Add(skill);
            }

            _playerView.AbilityViewCreated += OnAbilityViewCreated;
            _playerView.CreatedClassSkill += OnClassSkillsCreated;
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

        private void OnClassSkillsCreated(ClassAbilityData classAbilityData, ClassSkillButtonView classSkillButtonView)
        {
            Ability newAbility;
            newAbility = _abilityFactory.CreateClassSkill(classAbilityData, false);

            if (classAbilityData.TypeAbility == TypeAbility.Summon)
            {
                _abilityPresenterFactory.CreateSummonAbilityPresenter(newAbility, classSkillButtonView, _player.ShotPoint, _player, (classAbilityData as SummonAbilityData).Summon.Summon, _player.Pool);
            }
            
            if (classAbilityData.TypeAbility == TypeAbility.ThrowAxe)
            {
                _abilityPresenterFactory.CreateThrowAxePresenter(newAbility, classSkillButtonView, _player, (classAbilityData as ThrowAxeClassAbility).AxemMssile);
            }

            if (classAbilityData.TypeAbility == TypeAbility.JerkFront)
            {
                _abilityPresenterFactory.CreateJerkFrontAnillityPresenter(newAbility, classSkillButtonView, _player, (classAbilityData as JerkFrontAbilityData).PoolParticle);
            }

            if (classAbilityData.TypeAbility == TypeAbility.Rage)
            {
                int boostDamage = 0;
                float boosMoveSpeed = 0;
                int boosArmor = 0;

                foreach (CardParameter parameter in classAbilityData.Parameters[0].CardParameters)//��� �� ����� ������� ������� �����
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
        }

        private void OnAbilityViewCreated(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint)
        {
            Ability newAbility = _abilityFactory.Create(_abilityAttributeData, _currentAbilityLevel, _abilityCooldownReduction, _abilityDuration, _abilityDamage, true);

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

            return isFind;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}