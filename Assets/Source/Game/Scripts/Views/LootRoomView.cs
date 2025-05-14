using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LootRoomView : RoomView
    {
        private readonly System.Random _rnd = new();

        [SerializeField] private PlateDiscovery _plateDiscovery;
        [SerializeField] private int _minValue;
        [SerializeField] private int _maxValue;


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
            int curentReward = _rnd.Next(_maxValue, _maxValue);
            RewardSeted?.Invoke(curentReward);
           // SetRoomComplitIcon();
        }
    }
}