using System;

namespace Assets.Source.Game.Scripts.Characters
{
    public abstract class State
    {
        protected readonly StateMachine StateMashine;

        protected bool CanTransit = true;

        public State(StateMachine stateMashine)
        {
            StateMashine = stateMashine;
        }

        public event Action Attacking;
        public event Action AdditionalAttacking;
        public event Action SpetiallAttacking;
        public event Action Moving;
        public event Action SetedIdle;

        public virtual void EnterState() { }

        public virtual void ExitState() { }

        public virtual void UpdateState() { }

        protected void AttackEvent() => Attacking?.Invoke();

        protected void AdditionalAttackEvent() => AdditionalAttacking?.Invoke();

        protected void SpetiallAttackEvent() => SpetiallAttacking?.Invoke();

        protected void MoveEvent() => Moving?.Invoke();

        protected void SetIdleState() => SetedIdle?.Invoke();

        protected void OnAllowTransition() => CanTransit = true;
    }
}