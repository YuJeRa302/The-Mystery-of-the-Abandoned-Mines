using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [RequireComponent(typeof(Animator))]
    public class SummonAnimation : MonoBehaviour
    {
        private SummonStateMachineExample _summonStateMashine;
        private Animator _animator;

        public event Action Attacked;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            _summonStateMashine.MachineInitialized -= AddAnimationAction;

            foreach (var events in _summonStateMashine.MachineStates)
            {
                events.Value.Attacking -= OnAttack;
                events.Value.Moving -= OnMove;
                events.Value.AdditionalAttacking -= OnAdditionalAttack;
                events.Value.SpecialAttacking -= OnSpetialAttack;
                events.Value.SetedIdle -= OnIdle;
            }
        }

        public void Initialization(SummonStateMachineExample summonStateMashine)
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
            foreach (var events in _summonStateMashine.MachineStates)
            {
                events.Value.SetedIdle += OnIdle;
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

        private void OnIdle() => _animator.SetTrigger(AnimationMobName.IDLE_ANIMATION);
    }
}