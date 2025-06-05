using IJunior.TypedScenes;
using Lean.Localization;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts
{
    public class LevelObserver : MonoBehaviour, IGameLoopService, ICoroutineRunner
    {
        private readonly float _minCountRooms = 1;
        private readonly float _pauseValue = 0;
        private readonly float _resumeValue = 1;
        private readonly float _loadControlValue = 0.9f;

        [SerializeField] private RoomPlacer _roomPlacer;
        [SerializeField] private Pool _enemuPool;
        [Space(20)]
        [SerializeField] private Transform _spawnPlayerPoint;
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private CameraControiler _cameraControiler;
        [Space(20)]
        [SerializeField] private GamePanelsView[] _panels;
        [Space(20)]
        [SerializeField] private CardLoader _cardLoader;
        [SerializeField] private AudioPlayer _audioPlayerService;
        [Space(20)]
        [SerializeField] private GameObject _canvasLoader;
        [SerializeField] private LeanLocalization _leanLocalization;

        private bool _canSeeDoor;
        private EnemySpawner _enemySpawner;
        private TrapsSpawner _trapsSpawner;
        private PlayerFactory _playerFactory;
        private Player _player;
        private RoomView _currentRoom;
        private int _currentRoomLevel = 0;
        private int _countStages = 0;
        private int _currentStage = 0;
        private AbilityFactory _abilityFactory;
        private AbilityPresenterFactory _abilityPresenterFactory;
        private GamePanelsViewModel _gamePanelsViewModel;
        private GamePanelsModel _gamePanelsModel;
        private SaveAndLoader _saveAndLoad;
        private AsyncOperation _load;
        private TemporaryData _temporaryData;
        private bool _isWinGame;
        private bool _isGameInterrupted = true;

        public event Action<bool> GamePaused;
        public event Action<bool> GameEnded;
        public event Action<bool> GameResumed;
        public event Action StageCompleted;
        public event Action<int> LootRoomComplited;

        public CameraControiler CameraControiler => _cameraControiler;
        public WeaponData[] ContractWeaponDatas { get; private set; }
        public int CurrentRoomLevel => _currentRoomLevel;
        public int CountRooms { get; private set; }
        public int CountStages => _countStages;

        private void Awake()
        {
            _canvasLoader.gameObject.SetActive(false);
            _cameraControiler.ChengeConfiner(_roomPlacer.StartRoom);
        }

        private void OnDestroy()
        {
            RemoveEntities();
        }

        public void Initialize(TemporaryData temporaryData)
        {
            _temporaryData = temporaryData;
            RegisterServices();
            CreatePlayer(_temporaryData);
            InitializeLevelEntities(_temporaryData);
            CreateGamePanelEntities(_temporaryData);
            LoadGamePanels();
            AddListeners();
            _gamePanelsModel.OpenCardPanel();
        }

        public void ResumeByRewarded()
        {
            if (_temporaryData.IsGamePause == true)
                Time.timeScale = _pauseValue;
            else
                Time.timeScale = _resumeValue;

            GameResumed?.Invoke(_temporaryData.MuteStateSound);
        }

        public void PauseByRewarded()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(true);
        }

        public void ResumeByMenu()
        {
            Time.timeScale = _resumeValue;
            GameResumed?.Invoke(_temporaryData.MuteStateSound);
            _temporaryData.SetPauseGame(false);
        }

        public void PauseByMenu()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(_temporaryData.MuteStateSound);
            _temporaryData.SetPauseGame(true);
        }

        public void ResumeByFullscreenAd()
        {
            Time.timeScale = _resumeValue;
            StartCoroutine(LoadScreenLevel(Menu.LoadAsync(_temporaryData)));
            GameResumed?.Invoke(_temporaryData.MuteStateSound);
        }

        public void PauseByFullscreenAd()
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(true);
        }

        private void PauseGameByVisibilityWindow(bool state)
        {
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke(!state);
        }

        private void ResumeGameByVisibilityWindow(bool state)
        {
            if (_temporaryData.IsGamePause == true)
                Time.timeScale = _pauseValue;
            else
                Time.timeScale = _resumeValue;

            GameResumed?.Invoke(state);
        }

        private void CreateGamePanelEntities(TemporaryData temporaryData)
        {
            _gamePanelsModel = new GamePanelsModel(temporaryData, _player, this, _cardLoader, _audioPlayerService, _leanLocalization);
            _gamePanelsViewModel = new GamePanelsViewModel(_gamePanelsModel);
        }

        private void CreatePlayer(TemporaryData temporaryData)
        {
            _playerFactory = new PlayerFactory(
                this,
                _abilityFactory,
                _abilityPresenterFactory,
                _playerPrefab,
                _spawnPlayerPoint,
                _playerView,
                temporaryData,
                _audioPlayerService,
                out Player player);

            _player = player;
        }

        private void InitializeLevelEntities(TemporaryData temporaryData)
        {
            _canSeeDoor = _cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft.gameObject);
            _countStages = temporaryData.LevelData.CountStages;
            CountRooms = temporaryData.LevelData.CountRooms;

            if (temporaryData.CurrentLevelState.IsComplete)
                _currentStage = 0;
            else
                _currentStage = temporaryData.CurrentLevelState.CurrentCompleteStages;

            _trapsSpawner = new TrapsSpawner();
            _roomPlacer.Initialize(_currentRoomLevel, _canSeeDoor, CountRooms);
            LockBossRoom();
            _cardLoader.Initialize(_player);
            _enemySpawner = new EnemySpawner(_enemuPool, this, _player, _currentRoomLevel, _audioPlayerService);
            _cameraControiler.SetLookTarget(_player.transform);
            _saveAndLoad = new SaveAndLoader();
            _saveAndLoad.Initialize(temporaryData);
            ContractWeaponDatas = temporaryData.LevelData.IsContractLevel == true ? (temporaryData.LevelData as ContractLevelData).WeaponDatas : null;
        }

        private void AddListeners()
        {
            AddRoomListener();
            AddPanelListener();
            _roomPlacer.StartRoom.SetRoomStatus();
            _roomPlacer.StartRoom.RoomEntering += OnRoomEntering;
            _enemySpawner.EnemyDied += OnEnemyDied;
            _enemySpawner.AllEnemyRoomDied += OnRoomCompleted;
            _player.PlayerLevelChanged += OnPlayerLevelChanged;
            _player.PlayerDied += LoseGame;
            YandexGame.onVisibilityWindowGame += OnVisibilityWindowGame;
        }

        private void RemoveListeners()
        {
            RemoveRoomListener();
            RemovePanelListener();

            if (_roomPlacer != null)
                _roomPlacer.StartRoom.RoomEntering -= OnRoomEntering;

            if (_enemySpawner != null)
            {
                _enemySpawner.EnemyDied -= OnEnemyDied;
                _enemySpawner.AllEnemyRoomDied -= OnRoomCompleted;
            }

            if (_player != null)
            {
                _player.PlayerLevelChanged -= OnPlayerLevelChanged;
                _player.PlayerDied -= LoseGame;
            }

            YandexGame.onVisibilityWindowGame -= OnVisibilityWindowGame;
        }

        private void AddRoomListener()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.RoomEntering += OnRoomEntering;

                if (room == room as LootRoomView)
                {
                    (room as LootRoomView).RoomCompleted += OnRoomCompleted;
                    (room as LootRoomView).RewardSeted += OnRewardRoomEnded;
                }
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

        private void AddPanelListener()
        {
            foreach (var panel in _panels)
            {
                panel.PanelOpened += PauseByMenu;
                panel.PanelClosed += ResumeByMenu;
                panel.GameClosed += OnGameClosed;
                panel.RewardAdOpened += PauseByRewarded;
                panel.RewardAdClosed += ResumeByRewarded;
                panel.FullscreenAdOpened += PauseByFullscreenAd;
                panel.FullscreenAdClosed += ResumeByFullscreenAd;
            }
        }

        private void RemovePanelListener()
        {
            foreach (var panel in _panels)
            {
                if (panel == null)
                    continue;

                panel.PanelOpened -= PauseByMenu;
                panel.PanelClosed -= ResumeByMenu;
                panel.GameClosed -= OnGameClosed;
                panel.RewardAdOpened -= PauseByRewarded;
                panel.RewardAdClosed -= ResumeByRewarded;
                panel.FullscreenAdOpened -= PauseByFullscreenAd;
                panel.FullscreenAdClosed -= ResumeByFullscreenAd;
            }
        }

        private void LoadGamePanels()
        {
            foreach (var panel in _panels)
            {
                panel.Initialize(_gamePanelsViewModel);
            }
        }

        private void CloseAllGamePanels()
        {
            foreach (var panel in _panels)
                panel.gameObject.SetActive(false);
        }

        private void RegisterServices()
        {
            _abilityFactory = new AbilityFactory(this);
            _abilityPresenterFactory = new AbilityPresenterFactory(this, this);
        }

        private void StageComplete()
        {
            _currentStage++;

            if (_currentStage == _countStages)
                WinGame();
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
            LockBossRoom();
        }

        private void WinGame()
        {
            CloseAllGamePanels();
            _roomPlacer.Clear();
            _isWinGame = true;
            _isGameInterrupted = false;
            GameEnded?.Invoke(_isWinGame);
        }

        private void LoseGame()
        {
            CloseAllGamePanels();
            _roomPlacer.Clear();
            _isGameInterrupted = false;
            GameEnded?.Invoke(_isWinGame);
        }

        private void RemoveEntities()
        {
            RemoveListeners();
            _player.Remove();
            _enemySpawner.Dispose();
            _trapsSpawner.Dispose();
        }

        private void LockAllDoors()
        {
            foreach (var room in _roomPlacer.CreatedRooms)
            {
                room.LockRoom();
            }
        }

        private void LockBossRoom() 
        {
            BossRoomView bossRoom = null;

            foreach (RoomView room in _roomPlacer.CreatedRooms)
            {
                if (room as BossRoomView)
                    bossRoom = (room as BossRoomView);
            }

            if (CountRooms > _minCountRooms)
                bossRoom.LockRoom();
            else
                bossRoom.UnlockBossRoom(true);
        }

        private void UnlockAllDoors()
        {
            bool isAllRoomsComplete = _roomPlacer.CreatedRooms
            .Where(room => !(room is BossRoomView))
            .All(room => room.IsComplete);

            foreach (var room in _roomPlacer.CreatedRooms)
            {
                if (room == room as BossRoomView)
                     (room as BossRoomView).UnlockBossRoom(isAllRoomsComplete);
                else
                    room.UnlockRoom();
            }
        }

        private void OnVisibilityWindowGame(bool state)
        {
            if (state == true)
                ResumeGameByVisibilityWindow(state);
            else
                PauseGameByVisibilityWindow(state);
        }

        private void OnPlayerLevelChanged()
        {
            _gamePanelsModel.OpenCardPanel();
        }

        private void OnEnemyDied(Enemy enemy)
        {
            _player.GetReward(enemy);
        }

        private void OnRewardRoomEnded(int reward)
        {
            _player.GetLootRoomReward(reward);
            LootRoomComplited?.Invoke(reward);
        }

        private void OnGameClosed()
        {
            StartCoroutine(LoadScreenLevel(Menu.LoadAsync(_temporaryData)));
            GameEnded?.Invoke(false);
            _temporaryData.SaveProgress(_player, _isWinGame, _isGameInterrupted);
        }

        private void OnRoomEntering(RoomView room)
        {
            _currentRoom = room;
            _cameraControiler.ChengeConfiner(room);

            if (room.IsComplete == false)
            {
                if (room.EnemySpawnPoints.Length == 0)
                    _trapsSpawner.Initialize(room);
                else
                    _enemySpawner.Initialize(room);

                LockAllDoors();
            }
        }

        private void OnRoomCompleted()
        {
            if (_currentRoom != _currentRoom as BossRoomView) 
            {
                _currentRoom.SetComplete();
                UnlockAllDoors();
            }

            if (_currentRoom == _currentRoom as BossRoomView)
                StageComplete();
        }

        private IEnumerator LoadScreenLevel(AsyncOperation asyncOperation)
        {
            if (_load != null)
                yield break;

            _canvasLoader.gameObject.SetActive(true);
            _load = asyncOperation;
            _load.allowSceneActivation = false;

            while (_load.progress < _loadControlValue)
            {
                yield return null;
            }

            _load.allowSceneActivation = true;
            _load = null;
        }
    }
}