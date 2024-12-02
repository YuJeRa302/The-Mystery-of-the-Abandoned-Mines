using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAbilityCaster : MonoBehaviour
    {
        [SerializeField] private Player _player;
        private PlayerView _playerView;

        private AbilityAttributeData _abilityAttributeData;
        private List<Ability> _abilities = new();
        private int _abilityDuration = 0;
        private int _abilityDamage = 0;
        private int _abilityCooldownReduction = 0;

        public event Action<AbilityAttributeData, int> AbilityTaked;
        public event Action<Ability> AbilityRemoved;
        public event Action<Ability> AbilityUsed;
        public event Action<Ability> AbilityEnded;

        private void Awake()
        {
            _player.PlayerStats.AbilityDurationChanged += OnAbilityDurationChanged;
            _player.PlayerStats.AbilityDamageChanged += OnAbilityDamageChanged;
            _player.PlayerStats.AbilityCooldownReductionChanged += OnAbilityCooldownReductionChanged;
        }

        private void OnDestroy()
        {
            _player.PlayerStats.AbilityDurationChanged -= OnAbilityDurationChanged;
            _player.PlayerStats.AbilityDamageChanged -= OnAbilityDamageChanged;
            _player.PlayerStats.AbilityCooldownReductionChanged -= OnAbilityCooldownReductionChanged;
        }

        public void TakeAbility(CardView cardView)
        {
            if (cardView.CardData.AttributeData == null)
                return;

            _abilityAttributeData = cardView.CardData.AttributeData as AbilityAttributeData;

            if (TryGetAbility(_abilityAttributeData, out Ability ability))
                RemoveAbility(ability);

            AbilityTaked?.Invoke(_abilityAttributeData, cardView.CardState.CurrentLevel);
        }

        public void AddAbility(Ability ability)
        {
            ability.AbilityUsed += OnAbilityUsed;
            ability.AbilityEnded += OnAbilityEnded;
            _abilities.Add(ability);
        }

        private void RemoveAbility(Ability ability)
        {
            ability.AbilityUsed -= OnAbilityUsed;
            ability.AbilityEnded -= OnAbilityEnded;
            AbilityRemoved?.Invoke(ability);
            _abilities.Remove(ability);
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
                            if ((ability as AttackAbility).TypeAttackAbility == (abilityAttributeData as AttackAbilityData).TypeAttackAbility)
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