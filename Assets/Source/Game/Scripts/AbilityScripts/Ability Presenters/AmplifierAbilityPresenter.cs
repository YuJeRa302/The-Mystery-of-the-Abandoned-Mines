using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AmplifierAbilityPresenter : IAbilityStrategy, IAbilityPauseStrategy
    {
        private Ability _ability;
        private ParticleSystem _particleSystem;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            _ability = abilityEntitiesHolder.Ability;
            _particleSystem = abilityEntitiesHolder.ParticleSystem;
        }

        public void UsedAbility(Ability ability)
        {
            _particleSystem.Play();
        }

        public void EndedAbility(Ability ability)
        {
            _particleSystem.Stop();
        }

        public void PausedGame(bool state)
        {
        }

        public void ResumedGame(bool state)
        {
            _ability.Use();
        }
    }
}