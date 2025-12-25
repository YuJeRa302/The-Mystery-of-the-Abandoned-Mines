using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class PlayerAnimation : IDisposable
    {
        private readonly GamePauseService _gamePauseService;
        private readonly ICoroutineRunner _coroutineRunner;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private Coroutine _moveCorontine;
        private Player _player;
        private float _maxSpeed;
        private CompositeDisposable _disposables = new();

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
            if (_moveCorontine != null && _coroutineRunner != null)
                _coroutineRunner.StopCoroutine(_moveCorontine);

            RemoveListeners();
        }

        private void AttackAnimation()
        {
            _animator.SetTrigger(AnimationPlayerName.ATTACK_ANIMATION);
        }

        public void UseCoverAbility()
        {
            _animator.SetTrigger(AnimationPlayerName.COVERT_ANIMATION);
        }

        public void UsedAbilityEnd()
        {
            _animator.SetTrigger(AnimationPlayerName.END_ANIMATION);
        }

        private void AddListeners()
        {
            _gamePauseService.GamePaused += OnGamePaused;
            _gamePauseService.GameResumed += OnGameResumed;

            MessageBroker.Default
              .Receive<M_Attack>()
              .Subscribe(m => AttackAnimation())
              .AddTo(_disposables);
        }

        private void RemoveListeners()
        {
            if (_gamePauseService != null)
            {
                _gamePauseService.GamePaused -= OnGamePaused;
                _gamePauseService.GameResumed -= OnGameResumed;
            }

            if (_disposables != null)
                _disposables.Dispose();
        }

        private void OnGamePaused(bool state)
        {
            if (_moveCorontine != null && _coroutineRunner != null)
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
                _animator.SetFloat(AnimationPlayerName.MOVE_ANIMATION, _rigidbody.velocity.magnitude / _maxSpeed);
                yield return null;
            }
        }
    }
}