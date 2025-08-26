using Assets.Source.Game.Scripts.AbilityScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_AbilityViewCreat
    {
        private AbilityView _abilityView;
        private ParticleSystem _particleSystem;

        public M_AbilityViewCreat(AbilityView abilityView, ParticleSystem particleSystem)
        {
            _abilityView = abilityView;
            _particleSystem = particleSystem;
        }

        public AbilityView AbilityView => _abilityView;
        public ParticleSystem ParticleSystem => _particleSystem;
    }
}