using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class ClassAbilityView : MonoBehaviour
    {
        private readonly int _breakValue = -1;
        private readonly int _shiftValue = 1;
        private readonly string _upgradeNameText = "UpgradeName";
        private readonly string _buttonResetText = "ButtonReset";
        private readonly string _nameText = "Name";
        private readonly string _descriptionText = "DescriptionsAbility";

        [SerializeField] private LeanLocalizedText _description;
        [SerializeField] private LeanLocalizedText _nameAbility;
        [SerializeField] private LeanLocalizedText _namePanel;
        [SerializeField] private LeanLocalizedText _resetButtonText;
        [SerializeField] private Text _coinsText;
        [Space(20)]
        [SerializeField] private ClassAbilityDataView _classAbilityDataView;
        [SerializeField] private PlayerClassDataView _playerClassDataView;
        [SerializeField] private ClassAbilityStatsView _classAbilityStatsViewPrefab;
        [Space(20)]
        [SerializeField] private Image _abilityImage;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Transform _abilityClassContainer;
        [SerializeField] private Transform _classContainer;
        [SerializeField] private Transform _classAbilityStatsConteiner;
        [SerializeField] private Text _currentPrice;
        [Space(20)]
        [SerializeField] private List<PlayerClassData> _playerClassDatas;
        [Space(20)]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _resetButton;
        [Space(20)]
        [SerializeField] private GameObject _parameterPanel;
        [SerializeField] private GameObject _descriptionPanel;

        private List<PlayerClassDataView> _playerClassDataViews = new();
        private List<ClassAbilityDataView> _classAbilityDataViews = new();
        private List<ClassAbilityStatsView> _classAbilityStatsViews = new();
        private ClassAbilityViewModel _classAbilityViewModel;
        private IAudioPlayerService _audioPlayerService;

        private void OnDisable()
        {
            SetActiveSubPanel(false);
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        public void Initialize(ClassAbilityViewModel classAbilityViewModel, IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            _classAbilityViewModel = classAbilityViewModel;
            _coinsText.text = _classAbilityViewModel.GetCoins().ToString();
            _abilityImage.sprite = _defaultSprite;
            _nameAbility.TranslationName = _nameText;
            _description.TranslationName = _descriptionText;
            _namePanel.TranslationName = _upgradeNameText;
            _resetButtonText.TranslationName = _buttonResetText;
            AddListener();
            SetActiveSubPanel(false);
            gameObject.SetActive(false);
        }

        private void AddListener()
        {
            _classAbilityViewModel.InvokedShow += Show;
            _classAbilityViewModel.InvokedAbilityUpgrade += OnAbilityUpgraded;
            _classAbilityViewModel.InvokedAbilityReset += OnAbilityReseted;
            _backButton.onClick.AddListener(OnExitButtonClicked);
            _upgradeButton.onClick.AddListener(Upgrade);
            _resetButton.onClick.AddListener(ResetAbilities);
        }

        private void RemoveListener()
        {
            _classAbilityViewModel.InvokedShow -= Show;
            _classAbilityViewModel.InvokedAbilityUpgrade -= OnAbilityUpgraded;
            _classAbilityViewModel.InvokedAbilityReset -= OnAbilityReseted;
            _backButton.onClick.RemoveListener(OnExitButtonClicked);
            _upgradeButton.onClick.RemoveListener(Upgrade);
            _resetButton.onClick.RemoveListener(ResetAbilities);
        }

        private void CreateClassAbility(PlayerClassData playerClassData)
        {
            if (playerClassData == null)
                return;

            foreach (ClassAbilityData classAbilityData in playerClassData.ClassAbilityDatas)
            {
                ClassAbilityDataView view = Instantiate(_classAbilityDataView, _abilityClassContainer);
                ClassAbilityState classAbilityState = _classAbilityViewModel.GetClassAbilityState(classAbilityData);
                view.Initialize(classAbilityData, classAbilityState, _classAbilityViewModel, _audioPlayerService);
                view.AbilitySelected += OnAbilitySelected;
                _classAbilityDataViews.Add(view);
            }
        }

        private void CreateClass()
        {
            foreach (PlayerClassData playerClassData in _playerClassDatas)
            {
                PlayerClassDataView view = Instantiate(_playerClassDataView, _classContainer);
                view.Initialize(playerClassData, _audioPlayerService);
                view.PlayerClassSelected += OnPlayerClassSelected;
                _playerClassDataViews.Add(view);
            }
        }

        private void ClearClass()
        {
            if (_playerClassDataViews.Count == 0)
                return;

            foreach (PlayerClassDataView view in _playerClassDataViews)
            {
                view.PlayerClassSelected -= OnPlayerClassSelected;
                Destroy(view.gameObject);
            }

            _playerClassDataViews.Clear();
        }

        private void ClearClassAbility()
        {
            if (_classAbilityDataViews.Count == 0)
                return;

            foreach (ClassAbilityDataView view in _classAbilityDataViews)
            {
                view.AbilitySelected -= OnAbilitySelected;
                Destroy(view.gameObject);
            }

            _classAbilityDataViews.Clear();
        }

        private void OnPlayerClassSelected(PlayerClassDataView playerClassDataView)
        {
            ClearClassAbility();
            CreateClassAbility(playerClassDataView.PlayerClassData);
            _classAbilityViewModel.SelectPlayerClass(playerClassDataView.PlayerClassData);
        }

        private void OnAbilityUpgraded(ClassAbilityState classAbilityState)
        {
            _coinsText.text = _classAbilityViewModel.GetCoins().ToString();

            foreach (var view in _classAbilityDataViews)
            {
                if (view.ClassAbilityState.Id == classAbilityState.Id)
                {
                    OnAbilitySelected(view);
                }
            }
        }

        private void OnAbilityReseted(PlayerClassData playerClassData)
        {
            CreateClassAbility(playerClassData);
        }

        private void OnAbilitySelected(ClassAbilityDataView classAbilityDataView)
        {
            SetActiveSubPanel(true);

            _abilityImage.sprite = classAbilityDataView.ClassAbilityData.Icon;
            _nameAbility.TranslationName = classAbilityDataView.ClassAbilityData.Name;
            _description.TranslationName = classAbilityDataView.ClassAbilityData.Description;
            _classAbilityViewModel.SelectClassAbility(classAbilityDataView);


            foreach (var view in _classAbilityStatsViews)
            {
                Destroy(view.gameObject);
            }

            _classAbilityStatsViews.Clear();

            string nameParametr = string.Empty;
            string valueCurrentLvl = string.Empty;
            string valueNextLvl = string.Empty;

            if (classAbilityDataView.ClassAbilityState.CurrentLevel > 0)
            {
                _currentPrice.text = classAbilityDataView.ClassAbilityData.AbilityClassParameters[classAbilityDataView.ClassAbilityState.CurrentLevel - 1].Cost.ToString();

                foreach (var stats in classAbilityDataView.ClassAbilityData.Parameters[classAbilityDataView.ClassAbilityState.CurrentLevel - 1].CardParameters)
                {
                    if (CantDisplayParametr(stats.TypeParameter))
                        continue;

                    ClassAbilityStatsView statsView = Instantiate(_classAbilityStatsViewPrefab, _classAbilityStatsConteiner);
                    nameParametr = stats.TypeParameter.ToString();
                    valueCurrentLvl = stats.Value.ToString();
                    bool canShowNextLvlStats = classAbilityDataView.ClassAbilityState.CurrentLevel >= classAbilityDataView.ClassAbilityData.Parameters.Count;
                    _currentPrice.text = classAbilityDataView.ClassAbilityData.AbilityClassParameters[classAbilityDataView.ClassAbilityState.CurrentLevel - 1].Cost.ToString();

                    if (!canShowNextLvlStats)
                    {
                        _currentPrice.text = classAbilityDataView.ClassAbilityData.AbilityClassParameters[classAbilityDataView.ClassAbilityState.CurrentLevel].Cost.ToString();

                        foreach (var statsNextLvl in classAbilityDataView.ClassAbilityData.Parameters[classAbilityDataView.ClassAbilityState.CurrentLevel].CardParameters)
                        {
                            if (statsNextLvl.TypeParameter == stats.TypeParameter)
                            {
                                valueNextLvl = statsNextLvl.Value.ToString();
                            }
                        }
                    }

                    statsView.Initialize(nameParametr, valueCurrentLvl, valueNextLvl);

                    _classAbilityStatsViews.Add(statsView);
                }
            }
            else
            {
                _currentPrice.text = classAbilityDataView.ClassAbilityData.AbilityClassParameters[classAbilityDataView.ClassAbilityState.CurrentLevel].Cost.ToString();

                foreach (var stats in classAbilityDataView.ClassAbilityData.Parameters[classAbilityDataView.ClassAbilityState.CurrentLevel].CardParameters)
                {
                    if (CantDisplayParametr(stats.TypeParameter))
                        continue;

                    ClassAbilityStatsView statsView = Instantiate(_classAbilityStatsViewPrefab, _classAbilityStatsConteiner);
                    nameParametr = stats.TypeParameter.ToString();
                    valueCurrentLvl = stats.Value.ToString();

                    statsView.Initialize(nameParametr, valueCurrentLvl, valueNextLvl);

                    _classAbilityStatsViews.Add(statsView);
                }
            }
        }

        private bool CantDisplayParametr(TypeParameter typeParameter)
        {
            if (typeParameter == TypeParameter.AbilityValue || typeParameter == TypeParameter.TargetMoveSpeed)
                return true;
            else
                return false;
        }

        private void ResetAbilities()
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
            int currentUpgradeCoins = 0;

            foreach (ClassAbilityDataView view in _classAbilityDataViews)
            {
                for (int index = view.ClassAbilityState.CurrentLevel - _shiftValue; index > _breakValue; index--)
                {
                    currentUpgradeCoins += view.ClassAbilityData.AbilityClassParameters[index].Cost;
                }
            }

            _classAbilityViewModel.ResetAbilities(currentUpgradeCoins);
            _coinsText.text = _classAbilityViewModel.GetCoins().ToString();
            ClearClassAbility();
        }

        private void Upgrade()
        {
            _classAbilityViewModel.UpgradeAbility();
            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void Show()
        {
            gameObject.SetActive(true);
            ClearClass();
            CreateClass();
        }

        private void OnExitButtonClicked()
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
            gameObject.SetActive(false);
            ClearClassAbility();
            ClearClass();
            _abilityImage.sprite = _defaultSprite;
            _classAbilityViewModel.Hide();
        }

        private void SetActiveSubPanel(bool isActive)
        {
            _descriptionPanel.SetActive(isActive);
            _parameterPanel.SetActive(isActive);
        }
    }
}