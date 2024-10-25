using UnityEngine;

public class AttackState : State
{
    private float _distance;
    private Player _target;
    private Enemy _enemy;

    public AttackState(StateMashine stateMashine, Player target, Enemy enemy,float attackDistance) : base(stateMashine)
    {
        _target = target;
        _distance = attackDistance;
        _enemy = enemy;
    }

    public override void EnterState()
    {
        AttackEvent();
    }

    public override void UpdateState()
    {
        Vector3 directionToTarget = _enemy.transform.position - _target.transform.position;
        float distance = directionToTarget.magnitude;

        if (distance > _distance)
            _stateMashine.SetState<MoveState>();
    }
}