using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Factories;
using Assets.Source.Game.Scripts.GamePanels;
using Assets.Source.Game.Scripts.Menu;
using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Rooms;
using Assets.Source.Game.Scripts.Saves;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.SpawnersScripts;
using Assets.Source.Game.Scripts.Utility;
using Assets.Source.Game.Scripts.ViewModels;
using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Levels
{
    public class GameConstruct : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private RoomPlacer _roomPlacer;
        [SerializeField] private Pool _enemuPool;
        [Space(20)]
        [SerializeField] private Transform _spawnPlayerPoint;
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private CameraScripts.CameraController _cameraControiler;
        [Space(20)]
        [SerializeField] private List<GamePanelsView> _gamePanelsViews;
        [Space(20)]
        [SerializeField] private CardLoader _cardLoader;
        [SerializeField] private AudioPlayer _audioPlayerService;
        [Space(20)]
        [SerializeField] private GameObject _canvasLoader;
        [SerializeField] private LeanLocalization _leanLocalization;

        private TrapsSpawner _trapsSpawner;
        private EnemySpawner _enemySpawner;
        private SaveAndLoader _saveAndLoader;
        private PersistentDataService _persistentDataService;
        private RoomService _roomService;
        private GamePauseService _gamePauseService;
        private GameLoopService _gameLoopService;
        private GamePanelsService _gamePanelsService;
        private GamePanelsViewModel _gamePanelsViewModel;
        private GamePanelsModel _gamePanelsModel;
        private Player _player;
        private AbilityFactory _abilityFactory;
        private AbilityPresenterFactory _abilityPresenterFactory;
        private GameConfig _gameConfig;

        private void Awake()
        {
            _canvasLoader.gameObject.SetActive(false);
            ConstructGameEntities();
        }

        private void OnDestroy()
        {
            _enemySpawner.Dispose();
            _trapsSpawner.Dispose();
            _roomService.Dispose();
            _gameLoopService.Dispose();
            _player.Remove();
        }

        private void ConstructGameEntities()
        {
            _gameConfig = Resources.Load<GameConfig>(DataPath.GameConfigDataPath);
            _persistentDataService = new PersistentDataService();
            _saveAndLoader = new SaveAndLoader(_persistentDataService);
            _saveAndLoader.LoadDataFromPrefs();
            _trapsSpawner = new TrapsSpawner();
            var levelData = _gameConfig.GetLevelData(
                _persistentDataService.PlayerProgress.LevelService.CurrentLevelId);
            _enemySpawner = new EnemySpawner(_enemuPool, this, _audioPlayerService, levelData.Tier);
            _gamePanelsService = new GamePanelsService(_gamePanelsViews);
            _gamePauseService = new GamePauseService(_gamePanelsService, _persistentDataService);
            _abilityFactory = new AbilityFactory(this);
            _gameLoopService = new GameLoopService(this,
                _saveAndLoader, _gamePanelsService,
                _persistentDataService, _canvasLoader);
            _abilityPresenterFactory = new AbilityPresenterFactory(_gameLoopService, _gamePauseService, this);

            _roomService = new RoomService(
                _gamePanelsService,
                _roomPlacer,
                _cameraControiler,
                _enemySpawner,
                _trapsSpawner,
                levelData.CountRooms,
                levelData.CountStages);

            PlayerFactory playerFactory = new PlayerFactory(
                _abilityFactory,
                _abilityPresenterFactory,
                _persistentDataService,
                _gamePauseService,
                _gameConfig,
                _cameraControiler,
                _playerPrefab,
                _playerView,
                _spawnPlayerPoint,
                _audioPlayerService,
                out Player player);

            _player = player;
            _cardLoader.Initialize(_player);

            _gamePanelsModel = new GamePanelsModel(
                _roomService,
                _gamePauseService,
                _persistentDataService,
                _cardLoader,
                _player,
                levelData,
                _audioPlayerService,
                _leanLocalization);

            _gamePanelsViewModel = new GamePanelsViewModel(_gamePanelsModel);
            _enemySpawner.InitPlayerInstance(_player);
            _roomService.InitPlayerInstance(_player);
            _gameLoopService.InitGameEntities(_player, _roomService);
            _gamePanelsService.InitGamePanels(_gamePanelsViewModel);
        }
    }
}