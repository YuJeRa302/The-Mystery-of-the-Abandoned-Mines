using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.Saves;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Views;
using Lean.Localization;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts.Menu
{
    public class MainMenuBuilder : MonoBehaviour, ICoroutineRunner
    {
        private readonly float _resumeValue = 1;

        [SerializeField] private AudioPlayer _audioPlayer;
        [SerializeField] private SettingsView _settingsView;
        [SerializeField] private UpgradesView _upgradesView;
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private WeaponsView _weaponsView;
        [SerializeField] private ClassAbilityView _classAbilityView;
        [SerializeField] private KnowledgeBaseView _knowledgeBaseView;
        [SerializeField] private ConfigData _configData;
        [SerializeField] private LeaderboardView _leaderboardView;
        [Space(20)]
        [SerializeField] private LeanLocalization _leanLocalization;
        [Space(20)]
        [SerializeField] private GameObject _canvasLoader;

        private SaveAndLoader _saveAndLoad;
        private PersistentDataService _persistentDataService;

        private void Awake()
        {
            Time.timeScale = _resumeValue;
            Build();

            if (YG2.isGameplaying != true)
                InitYandexGameEntities();
        }

        private void Build()
        {
            _persistentDataService = new PersistentDataService();
            _saveAndLoad = new(_persistentDataService, _configData);

            if (_saveAndLoad.TryGetGameData())
                _saveAndLoad.LoadDataFromCloud();
            else
                _saveAndLoad.LoadDataFromConfig();

            CreateMenuEntities();
        }

        private void InitYandexGameEntities()
        {
            YG2.GameReadyAPI();
            YG2.GameplayStart();
        }

        private void CreateMenuEntities()
        {
            SettingsModel settingsModel = new SettingsModel(
                _persistentDataService,
                _leanLocalization,
                _audioPlayer);

            LevelsModel levelsModel = new LevelsModel(
                _persistentDataService,
                this,
                _saveAndLoad, 
                _canvasLoader);

            UpgradeModel upgradeModel = new UpgradeModel(_persistentDataService);
            WeaponsModel weaponsModel = new WeaponsModel(_persistentDataService);
            ClassAbilityModel classAbilityModel = new ClassAbilityModel(_persistentDataService);
            LeaderboardModel leaderboardModel = new LeaderboardModel(_persistentDataService);
            MenuModel menuModel = new MenuModel();

            _settingsView.Initialize(settingsModel, _audioPlayer);
            _upgradesView.Initialize(upgradeModel, _audioPlayer);
            _mainMenuView.Initialize(menuModel, _audioPlayer);
            _levelsView.Initialize(levelsModel, _audioPlayer);
            _weaponsView.Initialize(weaponsModel, _audioPlayer);
            _classAbilityView.Initialize(classAbilityModel, _audioPlayer);
            _knowledgeBaseView.Initialize(_audioPlayer);
            _leaderboardView.Initialize(leaderboardModel);
        }
    }
}