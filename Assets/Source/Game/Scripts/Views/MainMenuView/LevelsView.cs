using DG.Tweening;
using Lean.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class LevelsView : MonoBehaviour
    {
        private readonly int _indexUnlockContractButton = 1;
        private readonly string _chooseLevelType = "LevelType";
        private readonly string _chooseLevel = "ChooseLevel";
        private readonly string _choosePlayerClass = "ChoosePlayerClass";
        private readonly string _chooseWeapon = "ChooseWeapon";
        private readonly float _durationAnimation = 1f;
        private readonly Color _startColorAnimation = Color.red;
        private readonly Color _endColorAnimation = new Color(118f, 73f, 0);

        [SerializeField] private Image _levelImage;
        [SerializeField] private LeanLocalizedText _levelName;
        [SerializeField] private LeanLocalizedText _levelDescription;
        [SerializeField] private LeanLocalizedText _textHint;
        [Space(20)]
        [SerializeField] private WeaponDataView _weaponDataView;
        [SerializeField] private LevelDataView _levelDataView;
        [SerializeField] private PlayerClassDataView _playerClassDataView;
        [Space(20)]
        [SerializeField] private Transform _weaponsContainer;
        [SerializeField] private Transform _levelsContainer;
        [SerializeField] private Transform _classContainer;
        [Space(20)]
        [SerializeField] private List<WeaponData> _weaponDatas;
        [Space(10)]
        [SerializeField] private List<LevelData> _levelDatas;
        [Space(10)]
        [SerializeField] private List<LevelData> _contractLevelDatas;
        [Space(10)]
        [SerializeField] private List<PlayerClassData> _playerClassDatas;
        [Space(20)]
        [SerializeField] private ScrollRect _levelsScrollRect;
        [SerializeField] private ScrollRect _weaponsScrollRect;
        [SerializeField] private ScrollRect _playerClassScrollRect;
        [SerializeField] private ScrollRect _modsScrollRect;
        [Space(10)]
        [SerializeField] private GameObject _levelInfo;
        [SerializeField] private GameObject _dialogPanel;
        [SerializeField] private Text _goldTextDialogPanel;
        [SerializeField] private Text _currentPlayerGold;
        [Space(20)]
        [SerializeField] private Button _defaultLevelButton;
        [SerializeField] private Button _contractButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _cancelButton;
        [Space(20)]
        [SerializeField] private LeanLocalizedText _descriptionContract;
        [SerializeField] private string _keyLockContract;
        [SerializeField] private string _keyUnLockContract;

        private bool _isContractLevel = false;
        private LevelDataView _currentLevelDataView;
        private List<PlayerClassDataView> _playerClassDataViews = new ();
        private List<LevelDataView> _levelDataViews = new ();
        private List<WeaponDataView> _weaponDataViews = new ();
        private LevelsViewModel _levelsViewModel;
        private IAudioPlayerService _audioPlayerService;
        private bool _isLevelSelect = false;
        private bool _isPlayerClassShow = false;
        private bool _isLevelsShow = false;
        private bool _isWeaponSelect = false;

        public event Action<int> WeaponStateReseted;

        private void OnDestroy()
        {
            RemoveListener();
        }

        public void Initialize(LevelsViewModel levelsViewModel, IAudioPlayerService audioPlayerService)
        {
            _levelsViewModel = levelsViewModel;
            _audioPlayerService = audioPlayerService;
            // _contractButton.interactable = _levelsViewModel.TryUnlockContractButton(_indexUnlockContractButton);
            SetInteractableContract();
            AddListener();
            SortWeaponsByTier();
            SortContractsByTier();
            SortLevelsByTier();
            gameObject.SetActive(false);
        }

        private void AddListener()
        {
            _levelsViewModel.InvokedShow += Show;
            _backButton.onClick.AddListener(OnBackButtonClicked);
            _buyButton.onClick.AddListener(OnBuyButtonClick);
            _cancelButton.onClick.AddListener(OnCancelButtonClick);
            _contractButton.onClick.AddListener(ShowContractLevels);
            _defaultLevelButton.onClick.AddListener(ShowDefaultLevels);
            _nextButton.onClick.AddListener(OnNextButtonClicked);
        }


        private void RemoveListener()
        {
            _levelsViewModel.InvokedShow -= Show;
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _buyButton.onClick.RemoveListener(OnBuyButtonClick);
            _cancelButton.onClick.RemoveListener(OnCancelButtonClick);
            _contractButton.onClick.RemoveListener(ShowContractLevels);
            _defaultLevelButton.onClick.RemoveListener(ShowDefaultLevels);
            _nextButton.onClick.RemoveListener(OnNextButtonClicked);
        }

        private void SetInteractableContract()
        {
            _contractButton.interactable = _levelsViewModel.TryUnlockContractButton(_indexUnlockContractButton);

            if (_contractButton.interactable)
                _descriptionContract.TranslationName = _keyUnLockContract;
            else
                _descriptionContract.TranslationName = _keyLockContract;

        }

        private void CreateContractLevels()
        {
            foreach (LevelData levelData in _contractLevelDatas)
            {
                LevelDataView view = Instantiate(_levelDataView, _levelsContainer);
                _levelDataViews.Add(view);
                LevelState levelState = _levelsViewModel.GetLevelState(levelData);
                view.Initialize(levelData, levelState, _levelsViewModel, _audioPlayerService);
                view.LevelSelected += OnLevelSelected;
            }
        }

        private void CreateDefaultLevels()
        {
            foreach (LevelData levelData in _levelDatas)
            {
                LevelDataView view = Instantiate(_levelDataView, _levelsContainer);
                _levelDataViews.Add(view);
                LevelState levelState = _levelsViewModel.GetLevelState(levelData);
                view.Initialize(levelData, levelState, _levelsViewModel, _audioPlayerService);
                view.LevelSelected += OnLevelSelected;
            }
        }

        private void CreateClass()
        {
            foreach (PlayerClassData playerClassData in _playerClassDatas)
            {
                PlayerClassDataView view = Instantiate(_playerClassDataView, _classContainer);
                _playerClassDataViews.Add(view);
                view.Initialize(playerClassData, _audioPlayerService);
                view.PlayerClassSelected += OnPlayerClassSelected;
            }
        }

        private void CreateWeapons(TypePlayerClass typePlayerClass)
        {
            foreach (WeaponData weaponData in _weaponDatas)
            {
                if (typePlayerClass == weaponData.TypePlayerClass)
                {
                    WeaponState weaponState = _levelsViewModel.GetWeaponState(weaponData);

                    if (weaponState.IsUnlock == false)
                    {
                        continue;
                    }
                    else
                    {
                        WeaponDataView view = Instantiate(_weaponDataView, _weaponsContainer);
                        _weaponDataViews.Add(view);
                        view.Initialize(weaponData, weaponState, _audioPlayerService, this);
                        view.Equipped += OnWeaponEquipped;
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        private void SortContractsByTier()
        {
            _contractLevelDatas.Sort(delegate (LevelData x, LevelData y) { return x.Tier.CompareTo(y.Tier); });
        }

        private void SortLevelsByTier()
        {
            _levelDatas.Sort(delegate (LevelData x, LevelData y) { return x.Tier.CompareTo(y.Tier); });
        }

        private void SortWeaponsByTier()
        {
            _weaponDatas.Sort(delegate (WeaponData x, WeaponData y) { return x.Tier.CompareTo(y.Tier); });
        }

        private void ShowDefaultLevels()
        {
            _textHint.TranslationName = _chooseLevel;
            _isContractLevel = false;
            _isLevelsShow = true;
            _nextButton.interactable = false;
            _levelInfo.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            _levelsScrollRect.gameObject.SetActive(true);
            _modsScrollRect.gameObject.SetActive(false);
            _weaponsScrollRect.gameObject.SetActive(false);
            _playerClassScrollRect.gameObject.SetActive(false);
            _audioPlayerService.PlayOneShotButtonClickSound();
            ClearLevels();
            CreateDefaultLevels();
        }

        private void ShowContractLevels()
        {
            _textHint.TranslationName = _chooseLevel;
            _isContractLevel = true;
            _isLevelsShow = true;
            _nextButton.interactable = false;
            _levelInfo.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            _levelsScrollRect.gameObject.SetActive(true);
            _modsScrollRect.gameObject.SetActive(false);
            _weaponsScrollRect.gameObject.SetActive(false);
            _playerClassScrollRect.gameObject.SetActive(false);
            _audioPlayerService.PlayOneShotButtonClickSound();
            ClearLevels();
            CreateContractLevels();
        }

        private void ShowPlayerClass()
        {
            if (_isPlayerClassShow == true)
                return;

            _textHint.TranslationName = _choosePlayerClass;
            _isPlayerClassShow = true;
            _nextButton.interactable = false;
            _levelsScrollRect.gameObject.SetActive(false);
            _levelInfo.SetActive(false);
            _weaponsScrollRect.gameObject.SetActive(true);
            _playerClassScrollRect.gameObject.SetActive(true);
            ClearClass();
            CreateClass();
        }

        private void HideLevels() 
        {
            _levelImage.gameObject.SetActive(false);
            _textHint.TranslationName = _chooseLevelType;
            _nextButton.interactable = false;
            _isLevelSelect = false;
            _isLevelsShow = false;
            _levelsScrollRect.gameObject.SetActive(false);
            _levelInfo.SetActive(false);
            _modsScrollRect.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(false);
            ClearLevels();
        }

        private void HidePlayerClass()
        {
            _levelImage.gameObject.SetActive(false);
            _textHint.TranslationName = _chooseLevel;
            _isPlayerClassShow = false;
            _isWeaponSelect = false;
            _nextButton.interactable = false;
            _levelsScrollRect.gameObject.SetActive(true);
            _levelInfo.SetActive(true);
            _weaponsScrollRect.gameObject.SetActive(false);
            _playerClassScrollRect.gameObject.SetActive(false);
            ClearWeapons();
            ClearClass();
        }

        private void ClearLevels()
        {
            if (_levelDataViews.Count == 0)
                return;

            foreach (LevelDataView view in _levelDataViews)
            {
                view.LevelSelected += OnLevelSelected;
                Destroy(view.gameObject);
            }

            _levelDataViews.Clear();
        }

        private void ClearWeapons()
        {
            if (_weaponDataViews.Count == 0)
                return;

            foreach (WeaponDataView view in _weaponDataViews)
            {
                view.Equipped += OnWeaponEquipped;
                Destroy(view.gameObject);
            }

            _weaponDataViews.Clear();
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

        private void OnPlayerClassSelected(PlayerClassDataView playerClassDataView)
        {
            _textHint.TranslationName = _chooseWeapon;
            ClearWeapons();
            CreateWeapons(playerClassDataView.PlayerClassData.TypePlayerClass);
            _levelsViewModel.SelectClass(playerClassDataView);
            _nextButton.interactable = false;
        }

        private void OnWeaponEquipped(WeaponDataView weaponDataView)
        {
            WeaponStateReseted?.Invoke(weaponDataView.WeaponData.Id);
            _nextButton.interactable = true;
            _levelsViewModel.SelectWeapon(weaponDataView);
            _isWeaponSelect = true;
        }

        private void OnLevelSelected(LevelDataView levelDataView)
        {
            _levelImage.gameObject.SetActive(true);
            _levelImage.sprite = levelDataView.LevelData.Icon;
            _levelName.TranslationName = levelDataView.LevelData.TranslationName;
            _levelDescription.TranslationName = levelDataView.LevelData.TranslationDescription;
            _nextButton.interactable = true;
            _levelsViewModel.SelectLevel(levelDataView);
            _currentLevelDataView = levelDataView;
            _isLevelSelect = true;
        }

        private void OnBuyButtonClick() 
        {
            if (_levelsViewModel.TryBuyContract(_currentLevelDataView.LevelData.Cost))
                LoadLevel();
            else
                PlayCoinsAnimation();

            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void OnCancelButtonClick() 
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
            _dialogPanel.SetActive(false);
        }

        private void OnNextButtonClicked()
        {
            if (_isLevelSelect == true)
                ShowPlayerClass();

            if (_isContractLevel == true)
            {
                if (_isWeaponSelect == true)
                    ShowDialogPanel();
            }
            else 
            {
                if (_isWeaponSelect == true)
                    LoadLevel();
            }

            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void OnBackButtonClicked()
        {
            _audioPlayerService.PlayOneShotButtonClickSound();

            if (_isPlayerClassShow == true) 
            {
                HidePlayerClass();
                return;
            }

            if (_isLevelsShow == true) 
            {
                HideLevels();
                return;
            }

            gameObject.SetActive(false);
            _levelsViewModel.Hide();
        }

        private void PlayCoinsAnimation() 
        {
            _goldTextDialogPanel.transform.localScale = Vector3.zero;

            _goldTextDialogPanel.transform
                .DOScale(_durationAnimation, _durationAnimation)
                .SetEase(Ease.OutBounce)
                .SetLink(_goldTextDialogPanel.gameObject);
        }

        private void ShowDialogPanel() 
        {
            _dialogPanel.SetActive(true);
            _currentPlayerGold.text = _levelsViewModel.GetPlayerConins().ToString();
            _goldTextDialogPanel.text = _currentLevelDataView.LevelData.Cost.ToString();
        }

        private void Show() => gameObject.SetActive(true);

        private void LoadLevel() => _levelsViewModel.LoadLevel();
    }
}