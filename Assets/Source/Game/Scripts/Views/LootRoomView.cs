using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LootRoomView : RoomView
    {
        [SerializeField] private PlateDiscovery _plateDiscovery;

        public event Action RoomCompleted;

        private void Awake()
        {
            _plateDiscovery.PlateEntered += SetRoomComplete;
        }

        private void OnDestroy()
        {
            _plateDiscovery.PlateEntered -= SetRoomComplete;
        }

        private void SetRoomComplete()
        {
            RoomCompleted?.Invoke();
           // SetRoomComplitIcon();
        }
    }
}