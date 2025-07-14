using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Source.Game.Scripts.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Summon))]
    public class SummonStateMachineExample : MonoBehaviour
    {
        private Player _player;
        private NavMeshAgent _meshAgent;
        private StateMachine _stateMachine;
        private Summon _summon;

        public event Action MachineInitialized;

        public Dictionary<Type, State> MachineStates => _stateMachine.States;

        private void FixedUpdate()
        {
            if (_stateMachine != null)
                _stateMachine.UpdateStateMashine();
        }

        public void InitializeStateMashine(Player player)
        {
            _meshAgent = GetComponent<NavMeshAgent>();
            _summon = GetComponent<Summon>();
            _player = player;
            _stateMachine = new StateMachine();

            _stateMachine.AddState(new SummonIdleState(_stateMachine, _player, _summon));
            _stateMachine.AddState(new SummonMoveState(_stateMachine, _player, _summon, _meshAgent));
            _stateMachine.AddState(new SummonAttackState(_stateMachine, _summon));

            MachineInitialized?.Invoke();
            _stateMachine.SetState<SummonIdleState>();
        }

        public void ResetState()
        {
            _stateMachine.SetState<SummonIdleState>();
        }
    }
}