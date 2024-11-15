using System;
using System.Collections.Generic;

public class StateMashine
{
    private Dictionary<Type, State> _states = new Dictionary<Type, State>();
    private State CurrentState { get; set; }
    private Player _target;

    public StateMashine(Player player)
    {
        _target = player;
    }

    public Dictionary<Type, State> States => _states;

    public void AddState(State state)
    {
        _states.Add(state.GetType(), state);
    }

    public void SetState<T>() where T : State
    {
        var type = typeof(T);

        if (CurrentState != null && CurrentState.GetType() == type)
            return;

        if (_states.TryGetValue(type, out var newState))
        {
            CurrentState?.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }
    }

    public void UpdateStateMashine() => CurrentState?.UpdateState();
}