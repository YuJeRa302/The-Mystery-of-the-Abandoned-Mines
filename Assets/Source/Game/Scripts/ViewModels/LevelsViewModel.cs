using Assets.Source.Game.Scripts;
using System;

public class LevelsViewModel
{
    private readonly LevelsModel _levelsModel;
    private readonly MenuModel _menuModel;

    public LevelsViewModel(LevelsModel levelsModel, MenuModel menuModel) 
    {
        _levelsModel = levelsModel;
        _menuModel = menuModel;
        _menuModel.InvokedLevelsShow += () => InvokedShow?.Invoke();
        _menuModel.InvokedMainMenuShow += () => InvokedHide?.Invoke();
    }

    public event Action InvokedShow;
    public event Action InvokedHide;

    public void Show() => _menuModel.InvokeLevelsShow();
    public void Hide() => _menuModel.InvokeLevelsHide();

    public LevelState[] GetLevels() => _levelsModel.LevelStates;
    public LevelState GetLevelState(LevelData levelData) => _levelsModel.GetLevelState(levelData);
    public WeaponState GetWeaponState(WeaponData weaponData) => _levelsModel.GetWeaponState(weaponData);
    public void SelectLevel(LevelDataView levelDataView) => _levelsModel.SelectLevel(levelDataView);
    public void SelectClass(PlayerClassDataView playerClassDataView) => _levelsModel.SelectClass(playerClassDataView);
    public void SelectWeapon(WeaponDataView weaponDataView) => _levelsModel.SelectWeapon(weaponDataView);
    public void LoadLevel() => _levelsModel.LoadLevel();
}
