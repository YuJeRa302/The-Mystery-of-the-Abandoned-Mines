using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Source.Game.Scripts.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Enemy))]
    public class EnemyStateMachineExample : MonoBehaviour
    {
        private Player _target;
        private StateMachine _stateMashine;
        private Enemy _enemy;
        private NavMeshAgent _meshAgent;

        public event Action MachineInitialized;

        public Dictionary<Type, State> MachineStates => _stateMashine.States;

        private void FixedUpdate()
        {
            if (_stateMashine != null)
                _stateMashine.UpdateStateMashine();
        }

        private void OnDestroy()
        {
            _enemy.Stuned -= OnEnemyStuned;
            _enemy.StunEnded -= OnEnemyEndedStun;
        }

        public void InitializeStateMachine(Player target)
        {
            _meshAgent = GetComponent<NavMeshAgent>();
            _enemy = GetComponent<Enemy>();
            _enemy.Stuned += OnEnemyStuned;
            _enemy.StunEnded += OnEnemyEndedStun;
            _target = target;
            _stateMashine = new StateMachine();

            _stateMashine.AddState(new EnemyIdleState(_stateMashine, _target));
            _stateMashine.AddState(new EnemyMoveState(_stateMashine, _target, _meshAgent, _enemy));
            _stateMashine.AddState(new EnemyStunedState(_stateMashine));

            if (_enemy.TryGetComponent(out Boss boss))
            {
                _stateMashine.AddState(new BossAttackState(_stateMashine, _target, _enemy));

                if (_enemy.TryGetComponent(out GoldDragon goldDragon))
                {
                    _stateMashine.AddState(new AditionalAttackGoldDragon(_stateMashine, _target, _enemy));
                    _stateMashine.AddState(new SpecialAttackGoldDragon(_stateMashine, _enemy));
                }

                if (_enemy.TryGetComponent(out Beholder beholder))
                {
                    _stateMashine.AddState(new BeholderAdditionalAttackState(_stateMashine, _target, _enemy));
                    _stateMashine.AddState(new BeholderSpecialAttackState(_stateMashine, _enemy));
                }
            }
            else if (_enemy.TryGetComponent(out RangeEnemy rangeEnemy))
            {
                _stateMashine.AddState(new EnemyRangeAttackState(_stateMashine,
                    _target, _enemy, rangeEnemy.BulletSpawner));
            }
            else
            {
                _stateMashine.AddState(new EnemyAttackState(_stateMashine, _target, _enemy));
            }

            MachineInitialized?.Invoke();

            _stateMashine.SetState<EnemyIdleState>();
        }

        public void ResetState()
        {
            _stateMashine.SetState<EnemyIdleState>();
        }

        private void OnEnemyStuned()
        {
            _stateMashine.SetState<EnemyStunedState>();
        }

        private void OnEnemyEndedStun()
        {
            _stateMashine.SetState<EnemyIdleState>();
        }
    }
}