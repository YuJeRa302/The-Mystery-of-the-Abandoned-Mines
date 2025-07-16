using System;

namespace Assets.Source.Game.Scripts.Characters
{
    public abstract class State
    {
        private readonly StateMachine _stateMachine;

        private bool _canTransit = true;

        public State(StateMachine stateMashine)
        {
            _stateMachine = stateMashine;
        }

        protected StateMachine StateMachine => _stateMachine;
        protected bool CanTransit => _canTransit;

        public event Action Attacking;
        public event Action AdditionalAttacking;
        public event Action SpecialAttacking;
        public event Action Moving;
        public event Action SetedIdle;

        public virtual void EnterState() { }

        public virtual void ExitState() { }

        public virtual void UpdateState() { }

        protected void AttackEvent() => Attacking?.Invoke();

        protected void AdditionalAttackEvent() => AdditionalAttacking?.Invoke();

        protected void SpetiallAttackEvent() => SpecialAttacking?.Invoke();

        protected void MoveEvent() => Moving?.Invoke();

        protected void SetIdleState() => SetedIdle?.Invoke();

        protected void OnAllowTransition() => _canTransit = true;

        protected void SetTransitStatus(bool staus) => _canTransit = staus;
    }
}