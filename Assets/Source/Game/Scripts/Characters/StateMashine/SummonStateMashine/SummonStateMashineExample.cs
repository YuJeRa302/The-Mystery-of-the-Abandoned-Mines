using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Source.Game.Scripts.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Summon))]
    public class SummonStateMashineExample : MonoBehaviour
    {
        private Player _player;
        private NavMeshAgent _meshAgent;
        private StateMachine _stateMashine;
        private Summon _summon;

        public event Action MashineInitialized;

        public Dictionary<Type, State> MashineStates => _stateMashine.States;

        private void FixedUpdate()
        {
            if (_stateMashine != null)
                _stateMashine.UpdateStateMashine();
        }

        public void InitializeStateMashine(Player player)
        {
            _meshAgent = GetComponent<NavMeshAgent>();
            _summon = GetComponent<Summon>();
            _player = player;
            _stateMashine = new StateMachine();

            _stateMashine.AddState(new SummonIdleState(_stateMashine, _player, _summon));
            _stateMashine.AddState(new SummonMoveState(_stateMashine, _player, _summon, _meshAgent));
            _stateMashine.AddState(new SummonAttackState(_stateMashine, _summon));

            MashineInitialized?.Invoke();
            _stateMashine.SetState<SummonIdleState>();
        }

        public void ResetState()
        {
            _stateMashine.SetState<SummonIdleState>();
        }
    }
}