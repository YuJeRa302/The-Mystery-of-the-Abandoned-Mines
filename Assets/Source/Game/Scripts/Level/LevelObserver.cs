using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LevelObserver : MonoBehaviour
    {
        private const float CameraAriaXStandart = 0f;
        private const float CameraAriaXMaxSaze = 1f;

        [SerializeField] private RoomPlacer _roomPlacer;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private CameraControiler _cameraControiler;
        [SerializeField] private NavMeshSurface _navSurface;

        private Room _currentRoom;
        private int _currentRoomLevel = 0;
        private float _cameraAriaSizeCurrent;

        private void Awake()
        {
            _cameraControiler.ChengeConfiner(_roomPlacer.StartRoom);
            Initialize();
        }

        private void OnEnable()
        {
            _enemySpawner.AllEnemyRoomDied += OnEnemyRoomDied;
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        private void Initialize() 
        {
            if (_cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft) == false)
            {
                _cameraAriaSizeCurrent = CameraAriaXMaxSaze;
                Debug.Log(_cameraAriaSizeCurrent);
            }

            _cameraAriaSizeCurrent = CameraAriaXStandart;
            _roomPlacer.Initialize(_currentRoomLevel, _cameraAriaSizeCurrent);
            AddListener();
            _navSurface.BuildNavMesh();
        }

        private void AddListener() 
        {
            foreach (var room in _roomPlacer.CreatedRooms) 
            {
                room.RoomEntering += OnRoomEntering;
            }

            _roomPlacer.StartRoom.SetRoomStatus();//
            _roomPlacer.StartRoom.RoomEntering += OnRoomEntering;//test
        }

        private void RemoveListener()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.RoomEntering -= OnRoomEntering;
            }

            _roomPlacer.StartRoom.RoomEntering -= OnRoomEntering;
        }

        private void OnRoomEntering(Room room) 
        {
            _currentRoom = room;
            _cameraControiler.ChengeConfiner(room);

            if(room.IsComplete == false)
                _enemySpawner.Initialize(room.EnemySpawnPoints, room.RoomData.EnemyData, _currentRoomLevel);
        }

        private void OnEnemyRoomDied()
        {
            _currentRoom.SetComplete();
        }
    }
}