using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachineExample _enemyStateMachine;

        private Animator _animator;

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

        private void TryAttackPlayer() => Attacked?.Invoke();

        private void TryAdditionAtacked() => AdditionalAttacked?.Invoke();

        private void EndAnimation() => AnimationCompleted?.Invoke();

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

        private void OnMove() => _animator.SetTrigger(AnimationMobName.MOVE_ANIMATION);

        private void OnAttack() => _animator.SetTrigger(AnimationMobName.ATTACK_ANIMATION);

        private void OnAdditionalAttack() => _animator.SetTrigger(AnimationMobName.ADDITIONAL_ATTACK_ANIMATION);

        private void OnSpetialAttack() => _animator.SetTrigger(AnimationMobName.SPECIAL_ATTACK_ANIMATION);
    }
}