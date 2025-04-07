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
        [SerializeField] private ConfigData _configData;

        private MenuSaveAndLoad _saveAndLoad;
        private TemporaryData _temporaryData;

        public Action EnebleSave;

        private void Start()
        {
            if (_temporaryData == null)
            {
                Build();

            }
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
                if (_saveAndLoad.TryGetGameData(out GameInfo gameInfo))
                {
                    _temporaryData = new TemporaryData(gameInfo);
                }
                else
                {
                    _temporaryData = new TemporaryData(_configData);
                }
            }

            SettingsModel settingsModel = new SettingsModel(_temporaryData);
            LevelsModel levelsModel = new LevelsModel(_temporaryData, this);
            UpgradeModel upgradeModel = new UpgradeModel(_temporaryData);
            MenuModel menuModel = new MenuModel();
            WeaponsModel weaponsModel = new WeaponsModel(_temporaryData);
            ClassAbilityModel classAbilityModel = new ClassAbilityModel(_temporaryData);

            MainMenuViewModel mainMenuViewModel = new MainMenuViewModel(menuModel);
            SettingsViewModel settingsViewModel = new SettingsViewModel(settingsModel, menuModel);
            LevelsViewModel levelsViewModel = new LevelsViewModel(levelsModel, menuModel);
            UpgradeViewModel upgradeViewModel = new UpgradeViewModel(upgradeModel, menuModel);
            WeaponsViewModel weaponsViewModel = new WeaponsViewModel(weaponsModel, menuModel);
            ClassAbilityViewModel classAbilityViewModel = new ClassAbilityViewModel(classAbilityModel, menuModel);

            _settingsView.Initialize(settingsViewModel, _audioPlayer);
            _upgradesView.Initialize(upgradeViewModel, _audioPlayer);
            _mainMenuView.Initialize(mainMenuViewModel, _audioPlayer);
            _levelsView.Initialize(levelsViewModel, _audioPlayer);
            _weaponsView.Initialize(weaponsViewModel, _audioPlayer);
            _classAbilityView.Initialize(classAbilityViewModel, _audioPlayer);
            _saveAndLoad.Initialize(_temporaryData);
            _temporaryData.SetUpgradesData(_upgradesView.UpgradeDatas);
        }
    }
}