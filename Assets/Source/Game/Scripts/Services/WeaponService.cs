using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;

[Serializable]
public class WeaponService
{
    public int CurrentWeaponId;
    public List<WeaponState> WeaponStates = new ();

    public void SetWeaponStates(WeaponState[] weaponStates)
    {
        for (int index = 0; index < weaponStates.Length; index++)
        {
            WeaponStates.Add(weaponStates[index]);
        }
    }

    public void UnlockWeaponByData(WeaponData weaponData)
    {
        if (WeaponStates != null)
        {
            foreach (WeaponState weaponState in WeaponStates)
            {
                if (weaponState.Id == weaponData.Id)
                {
                    weaponState.IsUnlock = true;
                }
                else 
                {
                    WeaponState newWeaponState = InitWeaponState(weaponData);
                    WeaponStates.Add(new(newWeaponState.Id, newWeaponState.IsEquip, newWeaponState.IsUnlock == true));
                }
            }
        }
    }

    public WeaponState GetWeaponStateByData(WeaponData weaponData)
    {
        WeaponState weaponState = FindWeaponState(weaponData.Id);

        if (weaponState == null)
            weaponState = InitWeaponState(weaponData);

        return weaponState;
    }

    private WeaponState FindWeaponState(int id)
    {
        if (WeaponStates != null)
        {
            foreach (WeaponState weaponState in WeaponStates)
            {
                if (weaponState.Id == id)
                    return weaponState;
            }
        }

        return null;
    }

    private WeaponState InitWeaponState(WeaponData weaponData)
    {
        WeaponState weaponState = new (weaponData.Id, false, false);
        WeaponStates.Add(weaponState);
        return weaponState;
    }
}
