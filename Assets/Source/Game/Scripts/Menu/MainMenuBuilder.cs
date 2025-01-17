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

        private void Start()
        {
            Build();
        }

        private void OnDestroy()
        {

        }

        private void Build()
        {
            TemporaryData temporaryData = new TemporaryData(_configData);
            SettingsModel settingsModel = new SettingsModel(temporaryData);
            LevelsModel levelsModel = new LevelsModel(temporaryData, this);
            UpgradeModel upgradeModel = new UpgradeModel(temporaryData);
            MenuModel menuModel = new MenuModel();
            WeaponsModel weaponsModel = new WeaponsModel(temporaryData);
            ClassAbilityModel classAbilityModel = new ClassAbilityModel(temporaryData);

            MainMenuViewModel mainMenuViewModel = new MainMenuViewModel(menuModel);
            SettingsViewModel settingsViewModel = new SettingsViewModel(settingsModel, menuModel);
            LevelsViewModel levelsViewModel = new LevelsViewModel(levelsModel, menuModel);
            UpgradeViewModel upgradeViewModel = new UpgradeViewModel(upgradeModel, menuModel);
            WeaponsViewModel weaponsViewModel = new WeaponsViewModel(weaponsModel, menuModel);
            ClassAbilityViewModel classAbilityViewModel = new ClassAbilityViewModel(classAbilityModel, menuModel);

            _settingsView.Initialize(settingsViewModel, _audioPlayer);
            _upgradesView.Initialize(upgradeViewModel, _audioPlayer);
            _mainMenuView.Initialize(mainMenuViewModel);
            _levelsView.Initialize(levelsViewModel, _audioPlayer);
            _weaponsView.Initialize(weaponsViewModel, _audioPlayer);
            _classAbilityView.Initialize(classAbilityViewModel, _audioPlayer);
        }
    }
}