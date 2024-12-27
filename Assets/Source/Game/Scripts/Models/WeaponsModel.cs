using Assets.Source.Game.Scripts;

public class WeaponsModel
{
    private readonly TemporaryData _temporaryData;

    public WeaponsModel(TemporaryData temporaryData) 
    {
        _temporaryData = temporaryData;
    }

    public WeaponState GetWeaponState(WeaponData weaponData)
    {
        WeaponState weaponState = _temporaryData.GetWeaponState(weaponData.Id);

        if (weaponState == null)
            weaponState = InitWeaponState(weaponData);

        return weaponState;
    }

    private WeaponState InitWeaponState(WeaponData weaponData)
    {
        WeaponState weaponState = new ();
        weaponState.Id = weaponData.Id;
        weaponState.IsEquip = false;
        weaponState.IsUnlock = false;
        return weaponState;
    }
}