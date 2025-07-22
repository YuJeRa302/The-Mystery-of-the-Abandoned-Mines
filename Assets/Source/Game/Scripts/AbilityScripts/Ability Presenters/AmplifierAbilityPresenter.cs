using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AmplifierAbilityPresenter : AbilityPresenter
    {
        private readonly IGameLoopService _gameLoopService;

        private ParticleSystem _particleSystem;

        public AmplifierAbilityPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            GamePauseService gamePauseService,
            GameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner, ParticleSystem particleSystem) :
            base(ability, abilityView, 
                player, gamePauseService, 
                gameLoopService, coroutineRunner)
        {
            _particleSystem = particleSystem;
            AddListener();
        }

        protected override void OnAbilityUsed(Ability ability)
        {
            _particleSystem.Play();
        }

        protected override void OnAbilityEnded(Ability ability)
        {
            _particleSystem.Stop();
        }
    }
}