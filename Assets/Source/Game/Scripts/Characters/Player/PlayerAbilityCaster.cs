using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAbilityCaster : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private PlayerView _playerView;

        private AbilityAttributeData _abilityAttributeData;
        private List<Ability> _abilities = new();
        private int _abilityDuration = 0;
        private int _abilityDamage = 0;
        private int _abilityCooldownReduction = 0;
        private AbilityFactory _abilityFactory;
        private AbilityPresenterFactory _abilityPresenterFactory;
        private int _currentAbilityLevel;

        public event Action<AbilityAttributeData, int> AbilityTaked;
        public event Action<Ability> AbilityRemoved;
        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;

        private void Awake()
        {
            _player.PlayerStats.AbilityDurationChanged += OnAbilityDurationChanged;
            _player.PlayerStats.AbilityDamageChanged += OnAbilityDamageChanged;
            _player.PlayerStats.AbilityCooldownReductionChanged += OnAbilityCooldownReductionChanged;
            _playerView.AbilityViewCreated += OnAbilityViewCreated;
        }

        private void OnDestroy()
        {
            _player.PlayerStats.AbilityDurationChanged -= OnAbilityDurationChanged;
            _player.PlayerStats.AbilityDamageChanged -= OnAbilityDamageChanged;
            _player.PlayerStats.AbilityCooldownReductionChanged -= OnAbilityCooldownReductionChanged;
            _playerView.AbilityViewCreated -= OnAbilityViewCreated;
            DestroyAbilities();
        }

        public void Initialize(AbilityFactory abilityFactory, AbilityPresenterFactory abilityPresenterFactory) 
        {
            _abilityFactory = abilityFactory;
            _abilityPresenterFactory = abilityPresenterFactory;
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

        private void OnAbilityViewCreated(AbilityView abilityView, ParticleSystem particleSystem, Transform throwPoint)
        {
            Ability newAbility = _abilityFactory.Create(_abilityAttributeData, _currentAbilityLevel, _abilityCooldownReduction, _abilityDuration, _abilityDamage);

            if (_abilityAttributeData.TypeAbility != TypeAbility.AttackAbility)
                _abilityPresenterFactory.CreateAmplifierAbilityPresenter(newAbility, abilityView, particleSystem);
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

        private void OnAbilityDurationChanged(int value)
        {
            _abilityDuration = value;
        }

        private void OnAbilityDamageChanged(int value)
        {
            _abilityDamage = value;
        }

        private void OnAbilityCooldownReductionChanged(int value)
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
    }
}