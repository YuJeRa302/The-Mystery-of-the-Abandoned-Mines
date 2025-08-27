using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_LegendaryAbilityViewCreat
    {
        private AbilityView _abilityView;
        private ParticleSystem _particleSystem;
        private ActiveAbilityData _activeAbilityData;

        public M_LegendaryAbilityViewCreat(
            AbilityView abilityView,
            ParticleSystem particleSystem,
            ActiveAbilityData activeAbilityData)
        {
            _abilityView = abilityView;
            _particleSystem = particleSystem;
            _activeAbilityData = activeAbilityData;
        }

        public AbilityView AbilityView => _abilityView;
        public ParticleSystem ParticleSystem => _particleSystem;
        public ActiveAbilityData ActiveAbilityData => _activeAbilityData;
    }
}