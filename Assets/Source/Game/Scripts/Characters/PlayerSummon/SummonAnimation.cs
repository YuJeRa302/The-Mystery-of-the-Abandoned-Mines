using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [RequireComponent(typeof(Animator))]
    public class SummonAnimation : MonoBehaviour
    {
        private SummonStateMashineExample _summonStateMashine;
        private Animator _animator;
        private AnimationMobName _animationEnemy = new AnimationMobName();

        public event Action Attacked;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            _summonStateMashine.MashineInitialized -= AddAnimationAction;

            foreach (var events in _summonStateMashine.MashineStates)
            {
                events.Value.Attacking -= OnAttack;
                events.Value.Moving -= OnMove;
                events.Value.AdditionalAttacking -= OnAdditionalAttack;
                events.Value.SpetiallAttacking -= OnSpetialAttack;
                events.Value.SetedIdle -= OnIdle;
            }
        }

        public void Initialization(SummonStateMashineExample summonStateMashine)
        {
            _summonStateMashine = summonStateMashine;
            AddAnimationAction();
        }

        private void TryAttackEnemy()
        {
            Attacked?.Invoke();
        }

        private void AddAnimationAction()
        {
            foreach (var events in _summonStateMashine.MashineStates)
            {
                events.Value.SetedIdle += OnIdle;
                events.Value.Attacking += OnAttack;
                events.Value.Moving += OnMove;
                events.Value.AdditionalAttacking += OnAdditionalAttack;
                events.Value.SpetiallAttacking += OnSpetialAttack;
            }
        }

        private void OnMove() => _animator.SetTrigger(_animationEnemy.MoveAnimation);

        private void OnAttack() => _animator.SetTrigger(_animationEnemy.AttackAnimation);

        private void OnAdditionalAttack() => _animator.SetTrigger(_animationEnemy.AdditionalAttackAnimation);

        private void OnSpetialAttack() => _animator.SetTrigger(_animationEnemy.SpecialAttackAnimation);

        private void OnIdle() => _animator.SetTrigger(_animationEnemy.IdleAnimation);
    }
}