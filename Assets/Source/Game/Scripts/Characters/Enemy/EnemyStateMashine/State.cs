using System;

public abstract class State 
{
    protected readonly StateMashine _stateMashine;

    public event Action Attacking;
    public event Action Moving;
    public event Action TakedDamage;
    public event Action PlayerLose;

    public State(StateMashine stateMashine)
    {
        _stateMashine = stateMashine;
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void UpdateState() { }

    protected void AttackEvent() => Attacking?.Invoke();

    protected void MoveEvent() => Moving?.Invoke();

    protected void TakeDamageEvent() => TakedDamage?.Invoke();

    protected void EnemyWinEvent() => PlayerLose?.Invoke();
}