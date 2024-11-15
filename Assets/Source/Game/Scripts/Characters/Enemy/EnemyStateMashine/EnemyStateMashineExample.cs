using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Enemy))]
public class EnemyStateMashineExample : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _attackDistance;
    [SerializeField] private AttackState _attack;
    
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
        _stateMashine.AddState(new MoveState(_stateMashine, _target, _attackDistance, _speed, _meshAgent, _enemy));

        if(_enemy.TryGetComponent(out Boss boss))
        {
            _stateMashine.AddState(new BossAttackState(_stateMashine, _target, _enemy, _attackDistance, _enemy.AttackDelay, _enemy.Damage, boss.AdditionalAttackDelay, _enemy.AnimationStateController));//
            _stateMashine.AddState(new BossSpecialAttackState(_stateMashine));//
        }
        else if (_enemy.TryGetComponent(out RangeEnemy rangeEnemy))
        {
            _stateMashine.AddState(new RangeAttackState(_stateMashine, _target, _enemy, _attackDistance, _enemy.AttackDelay, _enemy.Damage, _enemy.AnimationStateController, rangeEnemy.BulletSpawner));
        }
        else
        {
            _stateMashine.AddState(new AttackState(_stateMashine, _target, _enemy, _attackDistance, _enemy.AttackDelay, _enemy.Damage, _enemy.AnimationStateController));
        }

        MashineInitialized?.Invoke();

        _stateMashine.SetState<IdleState>();
    }

    public void ResetState()
    {
        _stateMashine.SetState<IdleState>();
    }
}