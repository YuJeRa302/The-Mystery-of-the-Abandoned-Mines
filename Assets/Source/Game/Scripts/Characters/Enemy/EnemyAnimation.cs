using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private EnemyStateMashineExample _enemyStateMashine;

        private Animator _animator;
        private HashAnimationEnemy _animationEnemy = new HashAnimationEnemy();

        public event Action Attacked;
        public event Action AdditionalAttacked;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _enemyStateMashine.MashineInitialized += AddAnimationAction;
        }

        private void OnDestroy()
        {
            _enemyStateMashine.MashineInitialized -= AddAnimationAction;

            foreach (var events in _enemyStateMashine.MashineStates)
            {
                events.Value.Attacking -= OnAttack;
                events.Value.Moving -= OnMove;
                events.Value.TakedDamage -= OnTakeDamage;
                events.Value.PlayerLose -= OnWinGame;
                events.Value.AdditionalAttacking -= OnAdditionalAttack;
            }
        }

        public void TryAttackPlayer()
        {
           Attacked?.Invoke();
        }

        public void TryAdditionAtacked()
        {
            AdditionalAttacked?.Invoke();
        }

        private void AddAnimationAction()
        {
            foreach (var events in _enemyStateMashine.MashineStates)
            {
                events.Value.Attacking += OnAttack;
                events.Value.Moving += OnMove;
                events.Value.TakedDamage += OnTakeDamage;
                events.Value.PlayerLose += OnWinGame;
                events.Value.AdditionalAttacking += OnAdditionalAttack;
            }
        }

        private void OnMove() => _animator.SetTrigger(_animationEnemy.MoveAnimation);

        private void OnAttack() => _animator.SetTrigger(_animationEnemy.AttackAnimation);

        private void OnAdditionalAttack() => _animator.SetTrigger(_animationEnemy.AdditionalAttackAnimation);

        private void OnTakeDamage() => _animator.SetTrigger(_animationEnemy.TakeDamageAnimation);

        private void OnWinGame() => _animator.SetTrigger(_animationEnemy.WinDanceAnimation);
    }
}