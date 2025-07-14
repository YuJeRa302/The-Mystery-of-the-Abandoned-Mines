using Assets.Source.Game.Scripts.Items;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class WeaponDataView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private readonly int _firstLevelIndex = 0;

        [SerializeField] private Image _icon;
        [SerializeField] private Image _colorTierWeapon;
        [SerializeField] private Image _stateWeaponImage;
        [SerializeField] private Sprite _lockWeaponSprite;
        [SerializeField] private Sprite _equipWeaponSprite;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Button _button;

        private WeaponData _weaponData;
        private WeaponState _weaponState;
        private IAudioPlayerService _audioPlayerService;
        private LevelsView _levelsView;

        public event Action<WeaponDataView> Equipped;

        public WeaponData WeaponData => _weaponData;


        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnEquip);

            if (_levelsView != null)
                _levelsView.WeaponStateReseted -= ResetState;
        }

        public void Initialize(WeaponData weaponData, WeaponState weaponState,
            IAudioPlayerService audioPlayerService, LevelsView levelsView)
        {
            _audioPlayerService = audioPlayerService;
            _levelsView = levelsView;
            _weaponState = weaponState;
            _weaponData = weaponData;
            _button.onClick.AddListener(OnEquip);
            Fill(_weaponData);
            CheckWeaponState(_weaponState);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void ResetState(int id)
        {
            if (id != _weaponData.Id)
                _stateWeaponImage.sprite = _defaultSprite;
        }

        private void Fill(WeaponData weaponData)
        {
            _icon.sprite = weaponData.Icon;
            _colorTierWeapon.color = new Color(weaponData.TierColor.r,
                weaponData.TierColor.g, weaponData.TierColor.b);

            if (_levelsView != null)
                _levelsView.WeaponStateReseted += ResetState;

            if (_levelsView == null)
                _stateWeaponImage.gameObject.SetActive(false);
        }

        private void CheckWeaponState(WeaponState weaponState)
        {
            if (weaponState.Id == _firstLevelIndex)
                return;

            SetLevelState(weaponState);
        }

        private void SetLevelState(WeaponState weaponState)
        {
            if (weaponState.IsUnlock == false)
                _stateWeaponImage.sprite = _lockWeaponSprite;
            else
                _stateWeaponImage.sprite = weaponState.IsEquip == false ? _defaultSprite : _equipWeaponSprite;
        }

        private void OnEquip()
        {
            if (_levelsView != null)
                _stateWeaponImage.sprite = _equipWeaponSprite;

            Equipped?.Invoke(this);
        }
    }
}