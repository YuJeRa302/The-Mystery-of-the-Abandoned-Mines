using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAnimation : IDisposable
    {
        private readonly IGameLoopService _gameLoopService;
        private readonly GamePauseService _gamePauseService;
        private readonly ICoroutineRunner _coroutineRunner;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private float _maxSpeed;
        private Coroutine _moveCorontine;
        private Player _player;
        private HashAnimationPlayer _animationPlayer = new HashAnimationPlayer();

        public PlayerAnimation(
            Animator animator,
            Rigidbody rigidbody,
            float maxSpeed,
            PlayerClassData classData,
            Player player,
            ICoroutineRunner coroutineRunner,
            IGameLoopService gameLoopService)
        {
            _gameLoopService = gameLoopService;
            _coroutineRunner = coroutineRunner;
            _animator = animator;
            _rigidbody = rigidbody;
            _maxSpeed = maxSpeed;
            _animator.runtimeAnimatorController = classData.AnimatorController;
            _player = player;
            _moveCorontine = _coroutineRunner.StartCoroutine(PlayingAnimationMove());
            AddListeners();
        }

        public PlayerAnimation(
            Animator animator,
            Rigidbody rigidbody,
            float maxSpeed,
            PlayerClassData classData,
            Player player,
            ICoroutineRunner coroutineRunner,
            GamePauseService gamePauseService)
        {
            _gamePauseService = gamePauseService;
            _coroutineRunner = coroutineRunner;
            _animator = animator;
            _rigidbody = rigidbody;
            _maxSpeed = maxSpeed;
            _animator.runtimeAnimatorController = classData.AnimatorController;
            _player = player;
            _moveCorontine = _coroutineRunner.StartCoroutine(PlayingAnimationMove());
            AddListeners();
        }

        public void Dispose()
        {
            if (_moveCorontine != null)
                _coroutineRunner.StopCoroutine(_moveCorontine);

            RemoveListeners();
            GC.SuppressFinalize(this);
        }

        public void AttackAnimation()
        {
            _animator.SetTrigger(_animationPlayer.AttackAnimation);
        }

        public void UseCoverAbility()
        {
            _animator.SetTrigger(_animationPlayer.CoverAnimation);
        }

        public void UsedAbilityEnd()
        {
            _animator.SetTrigger(_animationPlayer.EndAnimation);
        }

        private void AddListeners()
        {
            _gameLoopService.GamePaused += OnGamePaused;
            _gameLoopService.GameResumed += OnGameResumed;

            _gamePauseService.GamePaused += OnGamePaused;
            _gamePauseService.GameResumed += OnGameResumed;
        }

        private void RemoveListeners() 
        {
            _gamePauseService.GamePaused -= OnGamePaused;
            _gamePauseService.GameResumed -= OnGameResumed;

            if (_gameLoopService != null)
            {
                _gameLoopService.GamePaused -= OnGamePaused;
                _gameLoopService.GameResumed -= OnGameResumed;
            }
        }

        private void OnGamePaused(bool state)
        {
            if (_moveCorontine != null)
                _coroutineRunner.StopCoroutine(_moveCorontine);
        }

        private void OnGameResumed(bool state)
        {
            if (_moveCorontine != null)
                _coroutineRunner.StopCoroutine(_moveCorontine);

            _moveCorontine = _coroutineRunner.StartCoroutine(PlayingAnimationMove());
        }

        private IEnumerator PlayingAnimationMove()
        {
            while (_player.isActiveAndEnabled)
            {
                _animator.SetFloat(_animationPlayer.MoveAnimation, _rigidbody.velocity.magnitude / _maxSpeed);
                yield return null;
            }
        }
    }
}