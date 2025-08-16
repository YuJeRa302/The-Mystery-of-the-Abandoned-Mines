using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Items;
using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Lean.Localization;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class WeaponsView : MonoBehaviour
    {
        private readonly string _typeDamageTranslationName = "TypeDamage";
        private readonly string _damageParameterName = "Damage";

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
        [SerializeField] private GameObject _parameterPanel;

        private List<WeaponStatsView> _weaponStatsViews = new();
        private List<PlayerClassDataView> _playerClassDataViews = new();
        private List<WeaponDataView> _weaponDataViews = new();
        private CompositeDisposable _disposables = new();
        private WeaponsModel _weaponsModel;
        private IAudioPlayerService _audioPlayerService;

        private void OnEnable()
        {
            _parameterPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_disposables != null)
                _disposables.Dispose();

            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        public void Initialize(WeaponsModel weaponsModel, IAudioPlayerService audioPlayerService)
        {
            _weaponsModel = weaponsModel;
            _audioPlayerService = audioPlayerService;
            _parameterPanel.SetActive(false);
            gameObject.SetActive(false);

            MessageBroker.Default
                .Receive<M_WeaponsShow>()
                .Subscribe(m => Show())
                .AddTo(_disposables);

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
                    WeaponState weaponState = _weaponsModel.GetWeaponState(weaponData);
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
            _parameterPanel.SetActive(true);
            ClearWeaponStats();
            _nameWeapon.TranslationName = weaponDataView.WeaponData.TranslationName;
            _weaponImage.sprite = weaponDataView.WeaponData.Icon;
            CreateWeaponStats(weaponDataView);
        }

        private void CreateWeaponStats(WeaponDataView weaponDataView)
        {
            var damageSource = weaponDataView.WeaponData.DamageSource;
            var weaponParameters = weaponDataView.WeaponData.WeaponParameters;

            InstantiateStatView(_typeDamageTranslationName, damageSource.TypeDamage.ToString(), true);
            InstantiateStatView(_damageParameterName, damageSource.Damage.ToString(), false);

            foreach (var param in damageSource.DamageParameters)
            {
                InstantiateStatView(param.TypeDamageParameter.ToString(), param.Value.ToString(), false);
            }

            foreach (var param in weaponParameters)
            {
                InstantiateStatView(param.SupportiveParameter.ToString(), param.Value.ToString(), false);
            }
        }

        private void InstantiateStatView(string name, string value, bool isHeader)
        {
            WeaponStatsView view = Instantiate(_weaponStatsView, _weaponStatsContainer);
            view.Initialize(name, value, isHeader);
            _weaponStatsViews.Add(view);
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
            MessageBroker.Default.Publish(new M_Hide());
        }
    }
}