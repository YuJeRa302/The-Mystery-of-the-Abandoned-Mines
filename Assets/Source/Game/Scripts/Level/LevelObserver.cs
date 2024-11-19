using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LevelObserver : MonoBehaviour
    {
        private readonly float _pauseValue = 0;
        private readonly float _resumeValue = 1;

        [SerializeField] private RoomPlacer _roomPlacer;
        [Space(20)]
        [SerializeField] private Player _player;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private CameraControiler _cameraControiler;
        [SerializeField] private NavMeshSurface _navSurface;
        [Space(20)]
        [SerializeField] private GamePanels[] _panels;

        private Room _currentRoom;
        private int _currentRoomLevel = 0;

        public event Action GamePaused;
        public event Action GameResumed;
        public event Action GameEnded;
        public event Action GameClosed;

        public PlayerView PlayerView => _playerView;

        private void Awake()
        {
            _cameraControiler.ChengeConfiner(_roomPlacer.StartRoom);
            Initialize();
            AddPanelListener();
            LoadGamePanels();
        }

        private void OnEnable()
        {
            _enemySpawner.AllEnemyRoomDied += OnEnemyRoomDied;
        }

        private void OnDestroy()
        {
            RemoveListener();
            RemovePanelListener();
        }

        private void Initialize()
        {
            _roomPlacer.Initialize(_currentRoomLevel);
            _player.PlayerStats.Initialize(10, null, this); // test
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

        private void AddPanelListener()
        {
            foreach (var panel in _panels)
            {
                panel.PanelOpened += PauseGame;
                panel.PanelClosed += ResumeGame;
            }
        }

        private void RemovePanelListener()
        {
            foreach (var panel in _panels)
            {
                panel.PanelOpened -= PauseGame;
                panel.PanelClosed -= ResumeGame;
            }
        }

        private void LoadGamePanels()
        {
            foreach (var panel in _panels)
                panel.Initialize(_player, this);
        }

        private void CloseAllGamePanels()
        {
            foreach (var panel in _panels)
                panel.gameObject.SetActive(false);
        }

        private void PauseGame()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke();
        }

        private void ResumeGame()
        {
            Time.timeScale = _resumeValue;
            GameResumed?.Invoke();
        }

        private void OnRoomEntering(Room room)
        {
            _currentRoom = room;
            _cameraControiler.ChengeConfiner(room);

            if (room.IsComplete == false)
            {
                _enemySpawner.Initialize(room.EnemySpawnPoints, room.RoomData.EnemyData, _currentRoomLevel);
                LockAllDoors();
            }
        }

        private void LockAllDoors()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.LockRoom();
            }
        }

        private void UnlockAllDoors()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.UnlockRoom();
            }
        }

        private void OnEnemyRoomDied()
        {
            _currentRoom.SetComplete();
            UnlockAllDoors();
        }
    }
}