using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AbilityPresenterFactory
    {
        private readonly IGameLoopService _gameLoopService;
        private readonly ICoroutineRunner _coroutineRunner;

        public AbilityPresenterFactory(IGameLoopService gameLoopService, ICoroutineRunner coroutineRunner) 
        {
            _gameLoopService = gameLoopService;
            _coroutineRunner = coroutineRunner;
        }

        public AttackAbilityPresenter CreateAttackAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            Transform throwPoint,
            ParticleSystem particleSystem,
            Spell spell) 
        {
            AttackAbilityPresenter attackAbilityPresenter = new (
                ability,
                abilityView,
                player,
                throwPoint,
                particleSystem,
                _gameLoopService,
                _coroutineRunner,
                spell);

            return attackAbilityPresenter;
        }

        public AmplifierAbilityPresenter CreateAmplifierAbilityPresenter(Ability ability, AbilityView abilityView, ParticleSystem particleSystem) 
        {
            AmplifierAbilityPresenter amplifierAbilityPresenter = new (ability, abilityView, particleSystem, _gameLoopService);
            return amplifierAbilityPresenter;
        }
    }
}