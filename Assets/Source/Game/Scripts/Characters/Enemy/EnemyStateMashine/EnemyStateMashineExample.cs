using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Source.Game.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Enemy))]
    public class EnemyStateMashineExample : MonoBehaviour
    {
        private Player _target;
        private StateMashine _stateMashine;
        private Enemy _enemy;
        private NavMeshAgent _meshAgent;

        public event Action MashineInitialized;

        public Dictionary<Type, State> MashineStates => _stateMashine.States;

        private void FixedUpdate()
        {
            if (_stateMashine != null)
                _stateMashine.UpdateStateMashine();
        }

        public void InitializeStateMashine(Player target)
        {
            _meshAgent = GetComponent<NavMeshAgent>();
            _enemy = GetComponent<Enemy>();
            _target = target;
            _stateMashine = new StateMashine(_target);

            _stateMashine.AddState(new IdleState(_stateMashine, _target));
            _stateMashine.AddState(new MoveState(_stateMashine, _target, _meshAgent, _enemy));

            if (_enemy.TryGetComponent(out Boss boss))
            {
                _stateMashine.AddState(new BossAttackState(_stateMashine, _target, _enemy));
                _stateMashine.AddState(new BossSpecialAttackState(_stateMashine));
            }
            else if (_enemy.TryGetComponent(out RangeEnemy rangeEnemy))
            {
                _stateMashine.AddState(new RangeAttackState(_stateMashine, _target, _enemy, rangeEnemy.BulletSpawner));
            }
            else
            {
                _stateMashine.AddState(new AttackState(_stateMashine, _target, _enemy));
            }

            MashineInitialized?.Invoke();

            _stateMashine.SetState<IdleState>();
        }

        public void ResetState()
        {
            _stateMashine.SetState<IdleState>();
        }
    }
}