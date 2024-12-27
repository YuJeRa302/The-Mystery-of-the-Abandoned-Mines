using System;

namespace Assets.Source.Game.Scripts
{
    public class WeaponsViewModel
    {
        private readonly WeaponsModel _weaponsModel;
        private readonly MenuModel _menuModel;

        public WeaponsViewModel(WeaponsModel weaponsModel, MenuModel menuModel)
        {
            _weaponsModel = weaponsModel;
            _menuModel = menuModel;
            _menuModel.InvokedWeaponsShow += () => InvokedShow?.Invoke();
            _menuModel.InvokedMainMenuShow += () => InvokedHide?.Invoke();
        }

        public event Action InvokedShow;
        public event Action InvokedHide;

        public void Hide() => _menuModel.InvokeWeaponsHide();

        public WeaponState GetWeaponState(WeaponData weaponData) => _weaponsModel.GetWeaponState(weaponData);
    }
}