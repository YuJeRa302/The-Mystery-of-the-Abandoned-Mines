using IJunior.TypedScenes;
using System;
using System.Collections;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts
{
    public class LevelObserver : MonoBehaviour, IGameLoopService, ICoroutineRunner
    {
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

        public event Action<bool> GameEnded;
        public event Action GameClosed;
        public event Action GamePaused;
        public event Action GameResumed;
        public event Action StageCompleted;
        public event Action<int> LootRoomComplited;

        public CameraControiler CameraControiler => _cameraControiler;
        public WeaponData[] ContractWeaponDatas { get; private set; }
        public int CurrentRoomLevel => _currentRoomLevel;
        public int CountRooms { get; private set; }
        public int CountStages => _countStages;
        public bool IsPaused => throw new NotImplementedException();

        private void Awake()
        {
            _cameraControiler.ChengeConfiner(_roomPlacer.StartRoom);
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
            _temporaryData = temporaryData;

            _canSeeDoor = _cameraControiler.TrySeeDoor(_roomPlacer.StartRoom.WallLeft.gameObject);
            CountRooms = temporaryData.LevelData.CountRooms;
            _countStages = temporaryData.LevelData.CountStages;
            bool isLevelComplit = temporaryData.CurrentLevelState.IsComplete;

            if (isLevelComplit)
                _currentStage = 0;
            else
                _currentStage = temporaryData.CurrentLevelState.CurrentCompleteStages;

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
                _playerView,
                temporaryData,
                out Player player);

            _player = player;
            _cardLoader.Initialize(_player);
            _enemySpawner = new EnemySpawner(_enemuPool, this, _player, _currentRoomLevel);
            _cameraControiler.SetLookTarget(_player.transform);
            _saveAndLoad = new SaveAndLoader();
            _saveAndLoad.Initialize(temporaryData);
            ContractWeaponDatas = temporaryData.LevelData.IsContractLevel == true ? (temporaryData.LevelData as ContractLevelData).WeaponDatas : null;
            CreateGamePanelEntities(_temporaryData);
            LoadGamePanels();
            AddListener();
        }

        public void ResumeByRewarded()
        {
            Time.timeScale = _resumeValue;
            GameResumed?.Invoke();
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

        private void OnVisibilityWindowGame(bool state)
        {
            if (state == true)
                ResumeGameByVisibilityWindow(state);
            else
                PauseGameByVisibilityWindow(state);
        }

        private void PauseGameByVisibilityWindow(bool state)
        {
            _audioPlayerService.MuteSound(state);
            Time.timeScale = _pauseValue;
            GamePaused?.Invoke();
        }

        private void ResumeGameByVisibilityWindow(bool state)
        {
            if (_temporaryData.IsGamePause == true)
                Time.timeScale = _pauseValue;
            else
                Time.timeScale = _resumeValue;

            _audioPlayerService.MuteSound(state);
            GameResumed?.Invoke();
        }

        private void CreateGamePanelEntities(TemporaryData temporaryData)
        {
            _gamePanelsModel = new GamePanelsModel(temporaryData, _player, this, _cardLoader, _audioPlayerService);
            _gamePanelsViewModel = new GamePanelsViewModel(_gamePanelsModel);
        }

        private void AddListener()
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

        private void RemoveListener()
        {
            RemoveRoomListener();
            RemovePanelListener();
            _roomPlacer.StartRoom.RoomEntering -= OnRoomEntering;
            _enemySpawner.EnemyDied -= OnEnemyDied;
            _enemySpawner.AllEnemyRoomDied -= OnRoomCompleted;
            _player.PlayerLevelChanged -= OnPlayerLevelChanged;
            _player.PlayerDied -= LoseGame;
            YandexGame.onVisibilityWindowGame -= OnVisibilityWindowGame;
        }

        private void OnPlayerLevelChanged()
        {
            _gamePanelsModel.OpenCardPanel();
        }

        private void OnEnemyDied(Enemy enemy)
        {
            _player.GetReward(enemy);
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

        private void AddPanelListener()
        {
            foreach (var panel in _panels)
            {
                panel.PanelOpened += PauseByMenu;
                panel.PanelClosed += ResumeByMenu;
                panel.GameClosed += OnGameClosed;
                panel.RewardAdClosed += ResumeByRewarded;
            }
        }

        private void OnRewardRoomEnded(int reward)
        {
            LootRoomComplited?.Invoke(reward);
        }

        private void OnGameClosed()
        {
            _temporaryData.SaveProgress(_player, false);
            StartCoroutine(LoadScreenLevel(Menu.LoadAsync(_temporaryData)));
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
                panel.GameClosed -= OnGameClosed;
                panel.RewardAdClosed -= ResumeByRewarded;
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
        }

        private void WinGame() 
        {
            CloseAllGamePanels();
            _roomPlacer.Clear();
            GameEnded?.Invoke(true);
        }

        private void LoseGame() 
        {
            CloseAllGamePanels();
            _roomPlacer.Clear();
            GameEnded?.Invoke(false);
        }

        private IEnumerator LoadScreenLevel(AsyncOperation asyncOperation)
        {
            if (_load != null)
                yield break;

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