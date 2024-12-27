using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LevelObserver : MonoBehaviour, IGameLoopService, ICoroutineRunner
    {
        private readonly float _pauseValue = 0;
        private readonly float _resumeValue = 1;

        [SerializeField] private RoomPlacer _roomPlacer;
        [SerializeField] private Pool _enemuPool;
        [Space(20)]
        [SerializeField] private PlayerInventory _playerInventory;
        [SerializeField] private Transform _spawnPlayerPoint;
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private PlayerClassData _classData; //test
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private CameraControiler _cameraControiler;
        [SerializeField] private NavMeshSurface _navSurface;
        [Space(20)]
        [SerializeField] private GamePanels[] _panels;
        [Space(20)]
        [SerializeField] private CardPanel _cardPanel;

        private EnemySpawner _enemySpawner;
        private TrapsSpawner _trapsSpawner;
        private PlayerFactory _playerFactory;
        private Player _player;
        private Room _currentRoom;
        private int _currentRoomLevel = 0;
        private AbilityFactory _abilityFactory;
        private AbilityPresenterFactory _abilityPresenterFactory;

        public event Action GameEnded;
        public event Action GameClosed;
        public event Action GamePaused;
        public event Action GameResumed;

        public PlayerView PlayerView => _playerView;
        public CameraControiler CameraControiler => _cameraControiler;

        public bool IsPaused => throw new NotImplementedException();

        private void Awake()
        {
            _cameraControiler.ChengeConfiner(_roomPlacer.StartRoom);
           // Initialize();
            AddPanelListener();
            LoadGamePanels();
        }

        private void OnDestroy()
        {
            RemoveListener();
            RemovePanelListener();

            _enemySpawner.Dispose();
            _trapsSpawner.Dispose();
        }

        public void Initialize(TemporaryData temporaryData)
        {
            bool canSeeDoor = _cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft);

            RegisterServices();
            _enemySpawner = new EnemySpawner(_enemuPool, this);
            _enemySpawner.AllEnemyRoomDied += OnEnemyRoomDied;
            _trapsSpawner = new TrapsSpawner();

            _roomPlacer.Initialize(_currentRoomLevel, canSeeDoor);
            _playerFactory = new PlayerFactory(_playerInventory, this, _abilityFactory, _abilityPresenterFactory, _playerPrefab, _spawnPlayerPoint, _classData, out Player player);
            _player = player;
            _playerView.Initialize(_player);
            _cameraControiler.SetLookTarget(_player.transform);
            _cardPanel.Initialize(_player);
            AddListener();
            _enemySpawner.SetTotalEnemyCount(_roomPlacer.AllEnemyCount, _player);
            _navSurface.BuildNavMesh();
        }

        public void ResumeByRewarded()
        {
            throw new NotImplementedException();
        }

        public void PauseByRewarded()
        {
            throw new NotImplementedException();
        }

        public void ResumeByMenu()
        {
            Time.timeScale = _resumeValue;
            GameResumed?.Invoke();
        }

        public void PauseByMenu()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke();
        }

        public void ResumeByInterstitial()
        {
            throw new NotImplementedException();
        }

        public void PauseByInterstitial()
        {
            throw new NotImplementedException();
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
                panel.PanelOpened += PauseByMenu;
                panel.PanelClosed += ResumeByMenu;
            }
        }

        private void RemovePanelListener()
        {
            foreach (var panel in _panels)
            {
                panel.PanelOpened -= PauseByMenu;
                panel.PanelClosed -= ResumeByMenu;
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

        private void OnRoomEntering(Room room)
        {
            _currentRoom = room;
            _cameraControiler.ChengeConfiner(room);

            if (room.IsComplete == false)
            {
                if(room.EnemySpawnPoints.Length == 0)
                    _trapsSpawner.Initialize(room);
                else
                    _enemySpawner.Initialize(room);

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

        private void RegisterServices()
        {
            _abilityFactory = new AbilityFactory(this);
            _abilityPresenterFactory = new AbilityPresenterFactory(this, this);
        }
    }
}