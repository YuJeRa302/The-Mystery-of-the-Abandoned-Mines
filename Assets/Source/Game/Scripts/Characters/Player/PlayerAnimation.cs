using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAnimation : IDisposable
    {
        private readonly IGameLoopService _gameLoopService;
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
            Player coroutineRunner,
            IGameLoopService gameLoopService)
        {
            _gameLoopService = gameLoopService;
            _coroutineRunner = coroutineRunner;
            _animator = animator;
            _rigidbody = rigidbody;
            _maxSpeed = maxSpeed;
            _animator.runtimeAnimatorController = classData.AnimatorController;
            _player = coroutineRunner;
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
        }

        private void RemoveListeners() 
        {
            _gameLoopService.GamePaused -= OnGamePaused;
            _gameLoopService.GameResumed -= OnGameResumed;
        }

        private void OnGamePaused()
        {
            if (_moveCorontine != null)
                _coroutineRunner.StopCoroutine(_moveCorontine);
        }

        private void OnGameResumed()
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