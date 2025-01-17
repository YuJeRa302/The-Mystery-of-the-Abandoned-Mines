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
        [SerializeField] private Transform _spawnPlayerPoint;
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private CameraControiler _cameraControiler;
        [SerializeField] private NavMeshSurface _navSurface;
        [Space(20)]
        [SerializeField] private GamePanels[] _panels;
        [Space(20)]
        [SerializeField] private CardPanel _cardPanel;

        private bool _canSeeDoor;
        private EnemySpawner _enemySpawner;
        private TrapsSpawner _trapsSpawner;
        private PlayerFactory _playerFactory;
        private Player _player;
        private RoomView _currentRoom;
        private int _currentRoomLevel = 0;
        private AbilityFactory _abilityFactory;
        private AbilityPresenterFactory _abilityPresenterFactory;
        private TemporaryData _temporaryData;//test

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
            //Initialize(_temporaryData); //test
            LoadGamePanels();
        }

        private void OnDestroy()
        {
            RemoveListener();
            _enemySpawner.Dispose();
            _trapsSpawner.Dispose();
        }

        public void Initialize(TemporaryData temporaryData)
        {
            RegisterServices();

            _canSeeDoor = _cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft);
            _enemySpawner = new EnemySpawner(_enemuPool, this);
            _trapsSpawner = new TrapsSpawner();
            _roomPlacer.Initialize(_currentRoomLevel, _canSeeDoor);

            _playerFactory = new PlayerFactory(
                temporaryData.WeaponData, 
                this, 
                _abilityFactory, 
                _abilityPresenterFactory,
                _playerPrefab, 
                _spawnPlayerPoint, 
                temporaryData.PlayerClassData, 
                out Player player);

            _player = player;
            _cardPanel.Initialize(_player);
            _playerView.Initialize(_player, temporaryData.PlayerClassData.Icon);
            _cameraControiler.SetLookTarget(_player.transform);
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
            AddRoomListener();
            AddPanelListener();
            _roomPlacer.StartRoom.SetRoomStatus();//
            _roomPlacer.StartRoom.RoomEntering += OnRoomEntering;//test
            _enemySpawner.AllEnemyRoomDied += OnRoomCompleted;
        }

        private void RemoveListener()
        {
            RemoveRoomListener();
            RemovePanelListener();
            _roomPlacer.StartRoom.RoomEntering -= OnRoomEntering;
            _enemySpawner.AllEnemyRoomDied -= OnRoomCompleted;
        }

        private void AddRoomListener() 
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.RoomEntering += OnRoomEntering;

                if (room == room as LootRoomView)
                    (room as LootRoomView).RoomCompleted += OnRoomCompleted;
            }
        }

        private void AddPanelListener()
        {
            foreach (var panel in _panels)
            {
                panel.PanelOpened += PauseByMenu;
                panel.PanelClosed += ResumeByMenu;
            }
        }

        private void RemoveRoomListener() 
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.RoomEntering -= OnRoomEntering;

                if (room == room as LootRoomView)
                    (room as LootRoomView).RoomCompleted -= OnRoomCompleted;
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

        private void OnRoomEntering(RoomView room)
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
                //if (room == room as BossRoomView)
                //    (room as BossRoomView).UnlockBossRoom(_roomPlacer.CreatedRooms.TrueForAll(completeRoom => completeRoom.IsComplete));
                //else
                room.UnlockRoom();
            }
        }

        private void OnRoomCompleted()
        {
            _currentRoom.SetComplete();
            UnlockAllDoors();

            if (_currentRoom == _currentRoom as BossRoomView)
                LevelCompleted();
        }

        private void RegisterServices()
        {
            _abilityFactory = new AbilityFactory(this);
            _abilityPresenterFactory = new AbilityPresenterFactory(this, this);
        }

        private void LevelCompleted() 
        {
            RemoveRoomListener();
            _roomPlacer.Clear();
            _currentRoomLevel++;
            _roomPlacer.Initialize(_currentRoomLevel, _canSeeDoor);
            AddRoomListener();
        }
    }
}