using Lean.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class LevelsView : MonoBehaviour
    {
        [SerializeField] private Image _levelImage;
        [SerializeField] private LeanLocalizedText _textNextButton;
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
        [Space(20)]
        [SerializeField] private Button _defaultLevelButton;
        [SerializeField] private Button _contractButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _nextButton;

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
            AddListener();
            SortElementsByTier();
            gameObject.SetActive(false);
        }

        private void AddListener()
        {
            _levelsViewModel.InvokedShow += Show;
            _backButton.onClick.AddListener(OnBackButtonClicked);
            _contractButton.onClick.AddListener(ShowContractLevels);
            _defaultLevelButton.onClick.AddListener(ShowDefaultLevels);
            _nextButton.onClick.AddListener(OnNextButtonClicked);
        }


        private void RemoveListener()
        {
            _levelsViewModel.InvokedShow -= Show;
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _contractButton.onClick.RemoveListener(ShowContractLevels);
            _defaultLevelButton.onClick.RemoveListener(ShowDefaultLevels);
            _nextButton.onClick.RemoveListener(OnNextButtonClicked);
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
                if (typePlayerClass == weaponData.TargetClass)
                {
                    WeaponState weaponState = _levelsViewModel.GetWeaponState(weaponData);

                    if (weaponState == null)
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

        private void SortElementsByTier()
        {
            _weaponDatas.Sort(delegate (WeaponData x, WeaponData y) { return y.Tier.CompareTo(x.Tier); });
        }

        private void ShowDefaultLevels()
        {
            _isLevelsShow = true;
            _nextButton.interactable = false;
            _levelInfo.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            _levelsScrollRect.gameObject.SetActive(true);
            _modsScrollRect.gameObject.SetActive(false);
            _weaponsScrollRect.gameObject.SetActive(false);
            _playerClassScrollRect.gameObject.SetActive(false);
            CreateDefaultLevels();
        }

        private void ShowContractLevels()
        {
            _isLevelsShow = true;
            _nextButton.interactable = false;
            _levelInfo.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            _levelsScrollRect.gameObject.SetActive(true);
            _modsScrollRect.gameObject.SetActive(false);
            _weaponsScrollRect.gameObject.SetActive(false);
            _playerClassScrollRect.gameObject.SetActive(false);
            CreateContractLevels();
        }

        private void ShowPlayerClass()
        {
            _isPlayerClassShow = true;
            _nextButton.interactable = false;
            _levelsScrollRect.gameObject.SetActive(false);
            _levelInfo.SetActive(false);
            _weaponsScrollRect.gameObject.SetActive(true);
            _playerClassScrollRect.gameObject.SetActive(true);
            CreateClass();
        }

        private void HideLevels() 
        {
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
            foreach (LevelDataView view in _levelDataViews)
            {
                view.LevelSelected += OnLevelSelected;
                Destroy(view.gameObject);
            }

            _levelDataViews.Clear();
        }

        private void ClearWeapons()
        {
            foreach (WeaponDataView view in _weaponDataViews)
            {
                view.Equipped += OnWeaponEquipped;
                Destroy(view.gameObject);
            }

            _weaponDataViews.Clear();
        }

        private void ClearClass()
        {
            foreach (PlayerClassDataView view in _playerClassDataViews)
            {
                view.PlayerClassSelected -= OnPlayerClassSelected;
                Destroy(view.gameObject);
            }

            _playerClassDataViews.Clear();
        }

        private void OnPlayerClassSelected(PlayerClassDataView playerClassDataView)
        {
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
            _nextButton.interactable = true;
            _levelsViewModel.SelectLevel(levelDataView);
            _isLevelSelect = true;
        }

        private void OnNextButtonClicked()
        {
            if (_isLevelSelect == true)
                ShowPlayerClass();

            if (_isWeaponSelect == true)
                LoadLevel();
        }

        private void OnBackButtonClicked()
        {
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

        private void Show() => gameObject.SetActive(true);

        private void LoadLevel() => _levelsViewModel.LoadLevel();
    }
}