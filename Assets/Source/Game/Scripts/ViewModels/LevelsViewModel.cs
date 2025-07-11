using System;

namespace Assets.Source.Game.Scripts
{
    public class LevelsViewModel
    {
        private readonly LevelsModel _levelsModel;
        private readonly MenuModel _menuModel;

        public LevelsViewModel(LevelsModel levelsModel, MenuModel menuModel)
        {
            _levelsModel = levelsModel;
            _menuModel = menuModel;
            _menuModel.InvokedLevelsShowed += () => Showing?.Invoke();
            _menuModel.InvokedMainMenuShowed += () => Hiding?.Invoke();
        }

        public event Action Showing;
        public event Action Hiding;

        public void Hide() => _menuModel.InvokeLevelsHide();
        public bool TryUnlockContractButton(int index) => _levelsModel.TryUnlockContractButton(index);
        public bool TryBuyContract(int cost) => _levelsModel.TryBuyContract(cost);
        public bool TryUnlockLevel(int levelId) => _levelsModel.TryUnlockLevel(levelId);
        public LevelState GetLevelState(LevelData levelData) => _levelsModel.GetLevelState(levelData);
        public WeaponState GetWeaponState(WeaponData weaponData) => _levelsModel.GetWeaponState(weaponData);
        public int GetPlayerConins() => _levelsModel.GetPlayerCoinCount();
        public void SelectLevel(LevelDataView levelDataView) => _levelsModel.SelectLevel(levelDataView);
        public void SelectClass(PlayerClassDataView playerClassDataView) => _levelsModel.SelectClass(playerClassDataView);
        public void SelectWeapon(WeaponDataView weaponDataView) => _levelsModel.SelectWeapon(weaponDataView);
        public void LoadLevel() => _levelsModel.LoadScene();
    }
}