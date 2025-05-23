using DG.Tweening;
using System;
using UnityEngine;
using YG;

namespace Assets.Source.Game.Scripts
{
    public class MainMenuBuilder : MonoBehaviour, ICoroutineRunner
    {
        private readonly float _pauseValue = 0;
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

        private SaveAndLoader _saveAndLoad;
        private TemporaryData _temporaryData;

        public Action EnebleSave;

        private void Start()
        {
            InitYandexGameEntities();
            AddListeners();

            if (_temporaryData == null)
                if(YandexGame.SDKEnabled)
                    Build();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
        
        public void Initialize(TemporaryData temporaryData)
        {
            Time.timeScale = 1;
            DOTween.Clear();
            DOTween.Init();
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

        private void AddListeners()
        {
            YandexGame.onVisibilityWindowGame += OnVisibilityWindowGame;
        }

        private void RemoveListeners() 
        {
            YandexGame.onVisibilityWindowGame -= OnVisibilityWindowGame;
        }

        private void InitYandexGameEntities()
        {
            YandexGame.GameplayStart();
            YandexGame.GameReadyAPI();
        }

        private void CreateMenuEntities() 
        {
            SettingsModel settingsModel = new SettingsModel(_temporaryData);
            LevelsModel levelsModel = new LevelsModel(_temporaryData, this);
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

        private void OnVisibilityWindowGame(bool state)
        {
            if (state == true)
                ResumeGame(state);
            else
                PauseGame(state);
        }

        private void ResumeGame(bool state)
        {
            if (_temporaryData.IsGamePause == true)
                Time.timeScale = _pauseValue;
            else
                Time.timeScale = _resumeValue;

            _audioPlayer.MuteSoundPayse(!state);
           // GameResumed?.Invoke();
        }

        private void PauseGame(bool state)
        {
            _audioPlayer.MuteSoundPayse(!state);
            Time.timeScale = _pauseValue;
           // GamePaused?.Invoke();
        }
    }
}