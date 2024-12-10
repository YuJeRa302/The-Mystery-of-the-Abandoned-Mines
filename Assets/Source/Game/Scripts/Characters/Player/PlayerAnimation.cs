using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAnimation : IDisposable
    {
        private ICoroutineRunner _coroutineRunner;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private float _maxSpeed;
        private Coroutine _moveCorontine;
        private Player _player;
        private HashAnimationPlayer _animationPlayer = new HashAnimationPlayer();

        public PlayerAnimation(Animator animator, Rigidbody rigidbody, float maxSpeed, PlayerClassData classData, Player coroutineRunner)
        {
            _animator = animator;
            _rigidbody = rigidbody;
            _maxSpeed = maxSpeed;
            _animator.runtimeAnimatorController = classData.AnimatorController;
            _coroutineRunner = coroutineRunner;
            _player = coroutineRunner;

            _moveCorontine = _coroutineRunner.StartCoroutine(PlayingAnimationMove());
        }

        public void Dispose()
        {
            if (_moveCorontine != null)
                _coroutineRunner.StopCoroutine(_moveCorontine);

            GC.SuppressFinalize(this);
        }

        public void AttackAnimation()
        {
            _animator.SetTrigger(_animationPlayer.AttackAnimation);
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