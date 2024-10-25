using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Enemy))]
public class EnemyStateMashineExample : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _attackDistance;
    [SerializeField] private Player _target;

    private StateMashine _stateMashine;
    private Enemy _enemy;
    private NavMeshAgent _meshAgent;

    public event Action MashineInitialized;

    public Dictionary<Type, State> MashineStates => _stateMashine.States;

    private void Start()
    {
        _meshAgent = GetComponent<NavMeshAgent>();
        _enemy = GetComponent<Enemy>();
        _stateMashine = new StateMashine(_target);

        _stateMashine.AddState(new IdleState(_stateMashine, _target));
        _stateMashine.AddState(new MoveState(_stateMashine, _target, _attackDistance, _speed, _meshAgent, _enemy));
        _stateMashine.AddState(new AttackState(_stateMashine, _target, _enemy, _attackDistance));

        MashineInitialized?.Invoke();

        _stateMashine.SetState<IdleState>();
    }

    private void Update()
    {
        if (_stateMashine != null)
            _stateMashine.UpdateStateMashine();
    }
}