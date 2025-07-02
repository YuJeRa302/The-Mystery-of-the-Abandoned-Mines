using Assets.Source.Game.Scripts;

public class WeaponsModel
{
    private readonly PersistentDataService _persistentDataService;

    public WeaponsModel(PersistentDataService persistentDataService) 
    {
        _persistentDataService = persistentDataService;
    }

    public WeaponState GetWeaponState(WeaponData weaponData)
    {
        return _persistentDataService.PlayerProgress.WeaponService.GetWeaponStateByData(weaponData);
    }
}