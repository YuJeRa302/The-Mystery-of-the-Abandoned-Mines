using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachineExample _enemyStateMachine;

        private Animator _animator;
        private AnimationMobName _animationEnemy = new AnimationMobName();

        public event Action Attacked;
        public event Action AdditionalAttacked;
        public event Action AnimationCompleted;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _enemyStateMachine.MachineInitialized += AddAnimationAction;
        }

        private void OnDestroy()
        {
            _enemyStateMachine.MachineInitialized -= AddAnimationAction;

            foreach (var events in _enemyStateMachine.MachineStates)
            {
                events.Value.Attacking -= OnAttack;
                events.Value.Moving -= OnMove;
                events.Value.AdditionalAttacking -= OnAdditionalAttack;
                events.Value.SpecialAttacking -= OnSpetialAttack;
            }
        }

        private void TryAttackPlayer()
        {
            Attacked?.Invoke();
        }

        private void TryAdditionAtacked()
        {
            AdditionalAttacked?.Invoke();
        }

        private void AddAnimationAction()
        {
            foreach (var events in _enemyStateMachine.MachineStates)
            {
                events.Value.Attacking += OnAttack;
                events.Value.Moving += OnMove;
                events.Value.AdditionalAttacking += OnAdditionalAttack;
                events.Value.SpecialAttacking += OnSpetialAttack;
            }
        }

        private void OnMove() => _animator.SetTrigger(_animationEnemy.MoveAnimation);

        private void OnAttack() => _animator.SetTrigger(_animationEnemy.AttackAnimation);

        private void OnAdditionalAttack() => _animator.SetTrigger(_animationEnemy.AdditionalAttackAnimation);

        private void OnSpetialAttack() => _animator.SetTrigger(_animationEnemy.SpecialAttackAnimation);
    }
}