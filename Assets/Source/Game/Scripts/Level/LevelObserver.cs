using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LevelObserver : MonoBehaviour
    {
        [SerializeField] private RoomPlacer _roomPlacer;
        [SerializeField] private EnemySpawner _enemySpawner;

        private int _currentRoomLevel = 0;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        private void Initialize() 
        {
            _roomPlacer.Initialize(_currentRoomLevel);
            AddListener();
        }

        private void AddListener() 
        {
            foreach (var room in _roomPlacer.CreatedRooms) 
            {
                room.RoomEntering += OnRoomEntering;
            }
        }

        private void RemoveListener()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.RoomEntering -= OnRoomEntering;
            }
        }

        private void OnRoomEntering(Room room) 
        {
            _enemySpawner.Initialize(room.EnemySpawnPoints, room.RoomData.EnemyData, _currentRoomLevel);
        }
    }
}