using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class WeaponsView : MonoBehaviour
    {
        private readonly string _typeDamageTranslationName = "TypeDamage";
        private readonly string _damageParametrName = "Damage";

        [SerializeField] private Image _weaponImage;
        [SerializeField] private Sprite _defaultSprite;
        [Space(20)]
        [SerializeField] private LeanLocalizedText _nameWeapon;
        [Space(20)]
        [SerializeField] private WeaponDataView _weaponDataView;
        [SerializeField] private PlayerClassDataView _playerClassDataView;
        [SerializeField] private WeaponStatsView _weaponStatsView;
        [Space(20)]
        [SerializeField] private Transform _weaponsContainer;
        [SerializeField] private Transform _classContainer;
        [SerializeField] private Transform _weaponStatsContainer;
        [Space(20)]
        [SerializeField] private List<WeaponData> _weaponDatas;
        [Space(10)]
        [SerializeField] private List<PlayerClassData> _playerClassDatas;
        [Space(20)]
        [SerializeField] private Button _backButton;
        [Space(20)]
        [SerializeField] private GameObject _parametrPanel;

        private List<WeaponStatsView> _weaponStatsViews = new ();
        private List<PlayerClassDataView> _playerClassDataViews = new ();
        private List<WeaponDataView> _weaponDataViews = new ();
        private WeaponsViewModel _weaponsViewModel;
        private IAudioPlayerService _audioPlayerService;

        private void OnEnable()
        {
            _parametrPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            _weaponsViewModel.InvokedShow -= Show;
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        public void Initialize(WeaponsViewModel weaponsViewModel, IAudioPlayerService audioPlayerService)
        {
            _weaponsViewModel = weaponsViewModel;
            _audioPlayerService = audioPlayerService;
            _parametrPanel.SetActive(false);
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
                if (typePlayerClass == weaponData.TypePlayerClass)
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
            _weaponDatas.Sort(delegate (WeaponData x, WeaponData y) { return x.Tier.CompareTo(y.Tier); });
        }

        private void ClearWeapons()
        {
            if (_weaponDataViews.Count == 0)
                return;

            foreach (WeaponDataView view in _weaponDataViews)
            {
                view.Equipped += OnWeaponSelected;
                Destroy(view.gameObject);
            }

            _weaponDataViews.Clear();
        }

        private void ClearWeaponStats()
        {
            if (_weaponStatsViews.Count == 0)
                return;

            foreach (WeaponStatsView view in _weaponStatsViews)
            {
                Destroy(view.gameObject);
            }

            _weaponStatsViews.Clear();
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
            ClearWeaponStats();
            ClearWeapons();
            CreateWeapons(playerClassDataView.PlayerClassData.TypePlayerClass);
        }

        private void OnWeaponSelected(WeaponDataView weaponDataView)
        {
            _parametrPanel.SetActive(true);
            ClearWeaponStats();
            _nameWeapon.TranslationName = weaponDataView.WeaponData.TranslationName;
            _weaponImage.sprite = weaponDataView.WeaponData.Icon;
            CreateWeaponStats(weaponDataView);
        }

        private void CreateWeaponStats(WeaponDataView weaponDataView) 
        {
            WeaponStatsView view = Instantiate(_weaponStatsView, _weaponStatsContainer);
            view.Initialize(_typeDamageTranslationName, weaponDataView.WeaponData.DamageSource.TypeDamage.ToString(), true);
            _weaponStatsViews.Add(view);
            WeaponStatsView damageView = Instantiate(_weaponStatsView, _weaponStatsContainer);
            damageView.Initialize(_damageParametrName, weaponDataView.WeaponData.DamageSource.Damage.ToString(), false);
            _weaponStatsViews.Add(damageView);

            foreach (var parametr in weaponDataView.WeaponData.DamageSource.DamageParameters)
            {
                view = Instantiate(_weaponStatsView, _weaponStatsContainer);
                view.Initialize(parametr.TypeDamageParameter.ToString(), parametr.Value.ToString(), false);
                _weaponStatsViews.Add(view);
            }

            foreach (var parametr in weaponDataView.WeaponData.WeaponParameters)
            {
                view = Instantiate(_weaponStatsView, _weaponStatsContainer);
                view.Initialize(parametr.SupportivePatametr.ToString(), parametr.Value.ToString(), false);
                _weaponStatsViews.Add(view);
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
            ClearClass();
            CreateClass();
            _weaponImage.sprite = _defaultSprite;
        }

        private void OnBackButtonClicked()
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
            ClearWeaponStats();
            ClearWeapons();
            ClearClass();
            gameObject.SetActive(false);
            _weaponsViewModel.Hide();
        }
    }
}