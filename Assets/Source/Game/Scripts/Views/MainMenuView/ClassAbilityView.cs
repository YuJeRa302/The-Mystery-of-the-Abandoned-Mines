using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.States;
using Lean.Localization;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
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
        [SerializeField] private Transform _classAbilityStatsContainer;
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

        private List<PlayerClassDataView> _playerClassDataViews = new ();
        private List<ClassAbilityDataView> _classAbilityDataViews = new ();
        private List<ClassAbilityStatsView> _classAbilityStatsViews = new ();
        private ClassAbilityModel _classAbilityModel;
        private IAudioPlayerService _audioPlayerService;
        private CompositeDisposable _disposables = new();

        private void OnDisable()
        {
            SetActiveSubPanel(false);
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        public void Initialize(ClassAbilityModel classAbilityModel, IAudioPlayerService audioPlayerService)
        {
            _classAbilityModel = classAbilityModel;
            _audioPlayerService = audioPlayerService;
            _coinsText.text = _classAbilityModel.Coins.ToString();
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
            MessageBroker.Default
                .Receive<M_ShowClassAbility>()
                .Subscribe(m => Show())
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityUpgraded>()
                .Subscribe(m => OnAbilityUpgraded(m.ClassAbilityState))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityReseted>()
                .Subscribe(m => OnAbilityReseted(m.PlayerClassData))
                .AddTo(_disposables);

            _backButton.onClick.AddListener(OnExitButtonClicked);
            _upgradeButton.onClick.AddListener(Upgrade);
            _resetButton.onClick.AddListener(ResetAbilities);
        }

        private void RemoveListener()
        {
            if (_disposables != null)
                _disposables.Dispose();

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
                ClassAbilityState classAbilityState = _classAbilityModel.GetClassAbilityState(classAbilityData);
                view.Initialize(classAbilityData, classAbilityState, _audioPlayerService);
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
            _classAbilityModel.SelectPlayerClass(playerClassDataView.PlayerClassData);
        }

        private void OnAbilityUpgraded(ClassAbilityState classAbilityState)
        {
            _coinsText.text = _classAbilityModel.Coins.ToString();

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

        private void ClearStats()
        {
            foreach (var view in _classAbilityStatsViews)
            {
                Destroy(view.gameObject);
            }

            _classAbilityStatsViews.Clear();
        }

        private void RenderCurrentAbility(ClassAbilityDataView view)
        {
            _abilityImage.sprite = view.ClassAbilityData.Icon;
            _nameAbility.TranslationName = view.ClassAbilityData.Name;
            _description.TranslationName = view.ClassAbilityData.Description;
        }

        private void OnAbilitySelected(ClassAbilityDataView classAbilityDataView)
        {
            SetActiveSubPanel(true);
            RenderCurrentAbility(classAbilityDataView);
            _classAbilityModel.SelectClassAbility(classAbilityDataView);
            ClearStats();

            int currentLevel = classAbilityDataView.ClassAbilityState.CurrentLevel;
            int parametersCount = classAbilityDataView.ClassAbilityData.Parameters.Count;
            bool isMaxLevel;

            if (currentLevel >= parametersCount)
                isMaxLevel = true;
            else
                isMaxLevel = false;

            UpdatePriceDisplay(classAbilityDataView, currentLevel, isMaxLevel);
            CreateAbilityStats(classAbilityDataView, currentLevel, isMaxLevel);
        }

        private void UpdatePriceDisplay(ClassAbilityDataView dataView, int currentLevel, bool isMaxLevel)
        {
            int priceLevel;

            if (currentLevel > 0)
                if (isMaxLevel)
                    priceLevel = currentLevel - 1;
                else
                    priceLevel = currentLevel;
            else
                priceLevel = 0;

            _currentPrice.text = dataView.ClassAbilityData.AbilityClassParameters[priceLevel].Cost.ToString();
        }

        private void CreateAbilityStats(ClassAbilityDataView dataView, int currentLevel, bool isMaxLevel)
        {
            int currentParamsIndex;

            if (currentLevel > 0)
                currentParamsIndex = currentLevel - 1;
            else
                currentParamsIndex = 0;

            var currentParams = dataView.ClassAbilityData.Parameters[currentParamsIndex].CardParameters;
            List<CardParameter> nextParams = null;

            currentParams = dataView.ClassAbilityData.Parameters[currentParamsIndex].CardParameters.ToList();

            if (!isMaxLevel)
                nextParams = dataView.ClassAbilityData.Parameters[currentLevel].CardParameters.ToList();

            for (int i = 0; i < currentParams.Count; i++)
            {
                var stat = currentParams[i];
                if (CantDisplayParameter(stat.TypeParameter.ToString())) 
                    continue;

                string nextValue = string.Empty;

                if (nextParams != null && currentLevel != 0)
                    nextValue = FindParameterValue(nextParams, stat.TypeParameter.ToString());

                CreateStatView(stat.TypeParameter.ToString(), stat.Value.ToString(), nextValue);
            }
        }

        private string FindParameterValue(List<CardParameter> parameters, string type)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].TypeParameter.ToString() == type.ToString())

                if (null == type)
                    return parameters[i].Value.ToString();
            }

            return string.Empty;
        }

        private void CreateStatView(string parameterName, string currentValue, string nextValue)
        {
            ClassAbilityStatsView statsView = Instantiate(_classAbilityStatsViewPrefab, _classAbilityStatsContainer);
            statsView.Initialize(parameterName, currentValue, nextValue);
            _classAbilityStatsViews.Add(statsView);
        }

        private bool CantDisplayParameter(string typeParameter)
        {
            if (typeParameter == TypeParameter.AbilityValue.ToString())
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

            _classAbilityModel.ResetAbilities(currentUpgradeCoins);
            _coinsText.text = _classAbilityModel.Coins.ToString();
            ClearClassAbility();
        }

        private void Upgrade()
        {
            _classAbilityModel.UpgradeAbility();
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
            MessageBroker.Default.Publish(new M_Hide());
        }

        private void SetActiveSubPanel(bool isActive)
        {
            _descriptionPanel.SetActive(isActive);
            _parameterPanel.SetActive(isActive);
        }
    }
}