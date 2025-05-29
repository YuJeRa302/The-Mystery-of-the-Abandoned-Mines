using Lean.Localization;
using System;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts
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
        private TemporaryData _temporaryData;

        public Action EnebleSave;

        private void Awake()
        {
            InitYandexGameEntities();
            Build();
        }
        
        public void Initialize(TemporaryData temporaryData)
        {
            Time.timeScale = _resumeValue;
            _temporaryData = temporaryData;
            Build();
        }

        private void Build()
        {
            _saveAndLoad = new();
            EnebleSave?.Invoke();

            if (_temporaryData == null)
            {
                if(_saveAndLoad.TryGetGameData(out SavesYG gameInfo))
                    _temporaryData = new TemporaryData(gameInfo);
                else
                    _temporaryData = new TemporaryData(_configData);
            }

            CreateMenuEntities();
            _saveAndLoad.Initialize(_temporaryData);
            _temporaryData.SetUpgradesData(_upgradesView.UpgradeDatas);
        }

        private void InitYandexGameEntities()
        {
            YandexGame.GameReadyAPI();
            YandexGame.GameplayStart();
        }

        private void CreateMenuEntities() 
        {
            SettingsModel settingsModel = new SettingsModel(_temporaryData, _leanLocalization, _audioPlayer);
            LevelsModel levelsModel = new LevelsModel(_temporaryData, this, _canvasLoader);
            UpgradeModel upgradeModel = new UpgradeModel(_temporaryData);
            MenuModel menuModel = new MenuModel();
            WeaponsModel weaponsModel = new WeaponsModel(_temporaryData);
            ClassAbilityModel classAbilityModel = new ClassAbilityModel(_temporaryData);
            KnowledgeBaseModel knowledgeBaseModel = new KnowledgeBaseModel();
            LeaderboardModel leaderboardModel = new LeaderboardModel(_temporaryData);

            MainMenuViewModel mainMenuViewModel = new MainMenuViewModel(menuModel);
            SettingsViewModel settingsViewModel = new SettingsViewModel(settingsModel, menuModel);
            LevelsViewModel levelsViewModel = new LevelsViewModel(levelsModel, menuModel);
            UpgradeViewModel upgradeViewModel = new UpgradeViewModel(upgradeModel, menuModel);
            WeaponsViewModel weaponsViewModel = new WeaponsViewModel(weaponsModel, menuModel);
            ClassAbilityViewModel classAbilityViewModel = new ClassAbilityViewModel(classAbilityModel, menuModel);
            KnowledgeBaseViewModel knowledgeBaseViewModel = new KnowledgeBaseViewModel(knowledgeBaseModel, menuModel);
            LeaderboardViewModel leaderboardViewModel = new LeaderboardViewModel(leaderboardModel, menuModel);

            _settingsView.Initialize(settingsViewModel, _audioPlayer);
            _upgradesView.Initialize(upgradeViewModel, _audioPlayer);
            _mainMenuView.Initialize(mainMenuViewModel, _audioPlayer);
            _levelsView.Initialize(levelsViewModel, _audioPlayer);
            _weaponsView.Initialize(weaponsViewModel, _audioPlayer);
            _classAbilityView.Initialize(classAbilityViewModel, _audioPlayer);
            _knowledgeBaseView.Initialize(knowledgeBaseViewModel, _audioPlayer);
            _leaderboardView.Initialize(leaderboardViewModel);
        }
    }
}