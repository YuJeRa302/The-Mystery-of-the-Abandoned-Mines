using System;
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
        [Space(20)]
        [SerializeField] private GamePanels[] _panels;

        private bool _canSeeDoor;
        private EnemySpawner _enemySpawner;
        private TrapsSpawner _trapsSpawner;
        private PlayerFactory _playerFactory;
        private Player _player;
        private RoomView _currentRoom;
        private int _currentRoomLevel = 0;
        private int _countStages = 0;
        private AbilityFactory _abilityFactory;
        private AbilityPresenterFactory _abilityPresenterFactory;

        public event Action GameEnded;
        public event Action GameClosed;
        public event Action GamePaused;
        public event Action GameResumed;
        public event Action StageCompleted;

        public PlayerView PlayerView => _playerView;
        public CameraControiler CameraControiler => _cameraControiler;
        public int CurrentRoomLevel => _currentRoomLevel;
        public int CountRooms { get; private set; }
        public int CountStages => _countStages;
        public bool IsPaused => throw new NotImplementedException();

        private void Awake()
        {
            _cameraControiler.ChengeConfiner(_roomPlacer.StartRoom);
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

            CountRooms = temporaryData.LevelData.CountRooms;
            _countStages = temporaryData.LevelData.CountStages;
            _canSeeDoor = _cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft.gameObject);
            _trapsSpawner = new TrapsSpawner();
            _roomPlacer.Initialize(_currentRoomLevel, _canSeeDoor, CountRooms);

            _playerFactory = new PlayerFactory(
                temporaryData.WeaponData, 
                this, 
                _abilityFactory, 
                _abilityPresenterFactory,
                _playerPrefab, 
                _spawnPlayerPoint, 
                temporaryData.PlayerClassData, 
                temporaryData, out Player player);

            _player = player;
            _enemySpawner = new EnemySpawner(_enemuPool, this, _player, _currentRoomLevel);
            _playerView.Initialize(_player, temporaryData.PlayerClassData.Icon);
            _player.PlayerAbilityCaster.Initialize();
            _cameraControiler.SetLookTarget(_player.transform);
            AddListener();
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
                if (room == null)
                    continue;

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
                StageComplete();
        }

        private void RegisterServices()
        {
            _abilityFactory = new AbilityFactory(this);
            _abilityPresenterFactory = new AbilityPresenterFactory(this, this);
        }

        private void StageComplete()
        {
            if (_currentRoomLevel == _countStages)
                EndGame();
            else
                CreateNextStage();
        }

        private void CreateNextStage() 
        {
            RemoveRoomListener();
            _roomPlacer.Clear();
            _player.transform.position = _spawnPlayerPoint.transform.position;
            _currentRoomLevel++;
            StageCompleted?.Invoke();
            _roomPlacer.Initialize(_currentRoomLevel, _canSeeDoor, CountRooms);
            AddRoomListener();
        }

        private void EndGame() 
        {
            CloseAllGamePanels();
            RemoveRoomListener();
            _roomPlacer.Clear();
            Debug.Log("END GAme");
        }
    }
}