using UnityEngine;
using UnityEngine.AI;

public class MoveState : State
{
    private Player _target;
    private float _moveSpeed;
    private float _distanceToTransition;
    private Enemy _enemy;
    private NavMeshAgent _navMeshAgent;

    public MoveState(StateMashine stateMashine, Player player, float distance, float moveSpeed, NavMeshAgent meshAgent, Enemy enemy) : base(stateMashine)
    {
        _target = player;
        _distanceToTransition = distance;
        _moveSpeed = moveSpeed;
        _navMeshAgent = meshAgent;
        _enemy = enemy;
    }

    public override void EnterState()
    {
        _navMeshAgent.speed = _moveSpeed;
        MoveEvent();
    }

    public override void UpdateState()
    {
        Vector3 directionToTarget = _enemy.transform.position - _target.transform.position;
        float distance = directionToTarget.magnitude;

        if (distance <= _distanceToTransition)
            _stateMashine.SetState<AttackState>();

        Move();
    }

    public override void ExitState()
    {
        _navMeshAgent.speed = 0;
    }

    private void Move()
    {
        _navMeshAgent.SetDestination(Vector3.forward + _target.transform.position);
    }
}