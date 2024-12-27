using Lean.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class WeaponsView : MonoBehaviour
    {
        [SerializeField] private Image _weaponImage;
        [SerializeField] private Sprite _defaultSprite;
        [Space(20)]
        [SerializeField] private LeanLocalizedText _nameWeapon;
        [SerializeField] private LeanLocalizedText _damageText;
        [SerializeField] private LeanLocalizedText _armorText;
        [SerializeField] private Text _damageValue;
        [SerializeField] private Text _armorValue;
        [Space(20)]
        [SerializeField] private WeaponDataView _weaponDataView;
        [SerializeField] private PlayerClassDataView _playerClassDataView;
        [Space(20)]
        [SerializeField] private Transform _weaponsContainer;
        [SerializeField] private Transform _classContainer;
        [Space(20)]
        [SerializeField] private List<WeaponData> _weaponDatas;
        [Space(10)]
        [SerializeField] private List<PlayerClassData> _playerClassDatas;
        [Space(20)]
        [SerializeField] private Button _backButton;

        private List<PlayerClassDataView> _playerClassDataViews = new ();
        private List<WeaponDataView> _weaponDataViews = new ();
        private WeaponsViewModel _weaponsViewModel;
        private IAudioPlayerService _audioPlayerService;

        private void OnDestroy()
        {
            _weaponsViewModel.InvokedShow -= Show;
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        public void Initialize(WeaponsViewModel weaponsViewModel, IAudioPlayerService audioPlayerService)
        {
            _weaponsViewModel = weaponsViewModel;
            _audioPlayerService = audioPlayerService;
            gameObject.SetActive(false);
            _weaponsViewModel.InvokedShow += Show;
            _backButton.onClick.AddListener(OnBackButtonClicked);
            SortElementsByTier();
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
                    WeaponState weaponState = _weaponsViewModel.GetWeaponState(weaponData);
                    WeaponDataView view = Instantiate(_weaponDataView, _weaponsContainer);
                    view.Initialize(weaponData, weaponState, _audioPlayerService, null);
                    view.Equipped += OnWeaponSelected;
                    _weaponDataViews.Add(view);
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

        private void ClearWeapons()
        {
            foreach (WeaponDataView view in _weaponDataViews)
            {
                view.Equipped += OnWeaponSelected;
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
        }

        private void OnWeaponSelected(WeaponDataView weaponDataView)
        {
            _nameWeapon.TranslationName = weaponDataView.WeaponData.TranslationName;
            _damageValue.text = weaponDataView.WeaponData.BonusDamage.ToString();
            _armorValue.text = weaponDataView.WeaponData.BonusArmor.ToString();
            _weaponImage.sprite = weaponDataView.WeaponData.Icon;
        }

        private void Show()
        {
            gameObject.SetActive(true);
            CreateClass();
            _nameWeapon.TranslationName = string.Empty;
            _damageValue.text = string.Empty;
            _armorValue.text = string.Empty;
            _weaponImage.sprite = _defaultSprite;
        }

        private void OnBackButtonClicked()
        {
            ClearWeapons();
            ClearClass();
            gameObject.SetActive(false);
            _weaponsViewModel.Hide();
        }
    }
}