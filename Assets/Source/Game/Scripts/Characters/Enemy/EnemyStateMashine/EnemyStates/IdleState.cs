using UnityEngine;

public class IdleState : State
{
    private Player _target;

    public IdleState(StateMashine stateMashine, Player player) : base(stateMashine)
    {
        _target = player;
    }

    public override void EnterState()
    {
        Debug.Log("Enter Idle State");
    }

    public override void UpdateState()
    {
        if(_target != null)
        {
            _stateMashine.SetState<MoveState>();
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exit Idle State");
    }
}