using System;

namespace Assets.Source.Game.Scripts
{
    public abstract class State
    {
        protected readonly StateMashine _stateMashine;

        protected bool _canTransit = true;

        public event Action Attacking;
        public event Action AdditionalAttacking;
        public event Action SpetiallAttacking;
        public event Action Moving;
        public event Action TakedDamage;
        public event Action PlayerLose;
        public event Action SetedIdle;

        public State(StateMashine stateMashine)
        {
            _stateMashine = stateMashine;
        }

        public virtual void EnterState() { }

        public virtual void ExitState() { }

        public virtual void UpdateState() { }

        protected void AttackEvent() => Attacking?.Invoke();

        protected void AdditionalAttackEvent() => AdditionalAttacking?.Invoke();

        protected void SpetiallAttackEvent() => SpetiallAttacking?.Invoke();

        protected void MoveEvent() => Moving?.Invoke();

        protected void TakeDamageEvent() => TakedDamage?.Invoke();

        protected void EnemyWinEvent() => PlayerLose?.Invoke();

        protected void SetIdleState() => SetedIdle?.Invoke();

        protected void OnAllowTransition() => _canTransit = true;
    }
}