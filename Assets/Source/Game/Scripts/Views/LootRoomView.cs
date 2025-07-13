using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Rooms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    public class LootRoomView : RoomView
    {
        private readonly System.Random _rnd = new();

        [SerializeField] private PlateDiscovery _plateDiscovery;
        [SerializeField] private int _minValue;
        [SerializeField] private int _maxValue;
        [SerializeField] private List<GameObject> _labyrinth;

        public event Action RoomCompleted;
        public event Action<int> RewardSeted;

        private void Awake()
        {
            _plateDiscovery.PlateEntered += SetRoomComplete;
        }

        private void OnDestroy()
        {
            _plateDiscovery.PlateEntered -= SetRoomComplete;
        }

        private void SetRoomComplete(Player player)
        {
            RoomCompleted?.Invoke();
            int curentReward = _rnd.Next(_minValue, _maxValue);
            RewardSeted?.Invoke(curentReward);
            RemoveLabyrinth();
        }

        private void RemoveLabyrinth()
        {
            foreach (var wall in _labyrinth)
            {
                wall.SetActive(false);
            }
        }
    }
}