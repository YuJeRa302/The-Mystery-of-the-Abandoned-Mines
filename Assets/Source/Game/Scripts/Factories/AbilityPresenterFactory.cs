using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Factories
{
    public class AbilityPresenterFactory
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly GameLoopService _gameLoopService;
        private readonly GamePauseService _gamePauseService;

        public AbilityPresenterFactory(GameLoopService gameLoopService,
            GamePauseService gamePauseService,
            ICoroutineRunner coroutineRunner)
        {
            _gameLoopService = gameLoopService;
            _gamePauseService = gamePauseService;
            _coroutineRunner = coroutineRunner;
        }
    }
}