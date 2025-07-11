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
            _menuModel.InvokedWeaponsShowed += () => Showing?.Invoke();
            _menuModel.InvokedMainMenuShowed += () => Hiding?.Invoke();
        }

        public event Action Showing;
        public event Action Hiding;

        public void Hide() => _menuModel.InvokeWeaponsHide();

        public WeaponState GetWeaponState(WeaponData weaponData) => _weaponsModel.GetWeaponState(weaponData);
    }
}