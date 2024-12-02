using System;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class LevelObserver : MonoBehaviour, IGameLoopService, ICoroutineRunner
    {
        private readonly float _pauseValue = 0;
        private readonly float _resumeValue = 1;

        [SerializeField] private RoomPlacer _roomPlacer;
        [Space(20)]
        [SerializeField] private PlayerFactory _playerFactory;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private CameraControiler _cameraControiler;
        [SerializeField] private NavMeshSurface _navSurface;
        [Space(20)]
        [SerializeField] private GamePanels[] _panels;
        [Space(20)]
        [SerializeField] private Button _pauseButton;//test buttons
        [SerializeField] private Button _resumeButton;//test

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
            bool canSeeDoor = _cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft);

            _roomPlacer.Initialize(_currentRoomLevel, canSeeDoor);
            _playerFactory.SpawnPlayer(out Player player);
            _player = player;
            _cameraControiler.SetLookTarget(_player.transform);
            _player.PlayerStats.Initialize(10, null, this); // test
            AddListener();
            _enemySpawner.SetTotalEnemyCount(_roomPlacer.AllEnemyCount, _player);
            RegisterServices();
            _player.PlayerStats.Initialize(10, null, this, _abilityFactory, _abilityPresenterFactory); // test
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

            _pauseButton.onClick.AddListener(PauseByMenu);//test
            _resumeButton.onClick.AddListener(ResumeByMenu);//test
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