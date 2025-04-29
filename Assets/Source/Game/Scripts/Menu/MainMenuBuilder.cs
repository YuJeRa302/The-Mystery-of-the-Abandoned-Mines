using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class MainMenuBuilder : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private AudioPlayer _audioPlayer;
        [SerializeField] private SettingsView _settingsView;
        [SerializeField] private UpgradesView _upgradesView;
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private WeaponsView _weaponsView;
        [SerializeField] private ClassAbilityView _classAbilityView;
        [SerializeField] private KnowledgeBaseView _knowledgeBaseView;
        [SerializeField] private ConfigData _configData;

        private SaveAndLoader _saveAndLoad;
        private TemporaryData _temporaryData;

        public Action EnebleSave;

        private async void Start()
        {
            if (_temporaryData == null)
                await Build();
        }

        private void OnEnable()
        {
            DOTween.Clear();
            DOTween.Init();
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }

        public async void Initialize(TemporaryData temporaryData)
        {
            Time.timeScale = 1;
            DOTween.Clear();
            DOTween.Init();
            _temporaryData = temporaryData;
            await Build();
        }

        private async UniTask Build()
        {
            _saveAndLoad = new();
            EnebleSave?.Invoke();

            if (_temporaryData == null)
            {
                var loadResult = await _saveAndLoad.TryGetGameData();
                _temporaryData = loadResult.Success
                    ? new TemporaryData(loadResult.Data)
                    : new TemporaryData(_configData);
            }

            SettingsModel settingsModel = new SettingsModel(_temporaryData);
            LevelsModel levelsModel = new LevelsModel(_temporaryData, this);
            UpgradeModel upgradeModel = new UpgradeModel(_temporaryData);
            MenuModel menuModel = new MenuModel();
            WeaponsModel weaponsModel = new WeaponsModel(_temporaryData);
            ClassAbilityModel classAbilityModel = new ClassAbilityModel(_temporaryData);
            KnowledgeBaseModel knowledgeBaseModel = new KnowledgeBaseModel();

            MainMenuViewModel mainMenuViewModel = new MainMenuViewModel(menuModel);
            SettingsViewModel settingsViewModel = new SettingsViewModel(settingsModel, menuModel);
            LevelsViewModel levelsViewModel = new LevelsViewModel(levelsModel, menuModel);
            UpgradeViewModel upgradeViewModel = new UpgradeViewModel(upgradeModel, menuModel);
            WeaponsViewModel weaponsViewModel = new WeaponsViewModel(weaponsModel, menuModel);
            ClassAbilityViewModel classAbilityViewModel = new ClassAbilityViewModel(classAbilityModel, menuModel);
            KnowledgeBaseViewModel knowledgeBaseViewModel = new KnowledgeBaseViewModel(knowledgeBaseModel, menuModel);

            _settingsView.Initialize(settingsViewModel, _audioPlayer);
            _upgradesView.Initialize(upgradeViewModel, _audioPlayer);
            _mainMenuView.Initialize(mainMenuViewModel, _audioPlayer);
            _levelsView.Initialize(levelsViewModel, _audioPlayer);
            _weaponsView.Initialize(weaponsViewModel, _audioPlayer);
            _classAbilityView.Initialize(classAbilityViewModel, _audioPlayer);
            _knowledgeBaseView.Initialize(knowledgeBaseViewModel, _audioPlayer);
            _saveAndLoad.Initialize(_temporaryData);
            _temporaryData.SetUpgradesData(_upgradesView.UpgradeDatas);
        }
    }
}