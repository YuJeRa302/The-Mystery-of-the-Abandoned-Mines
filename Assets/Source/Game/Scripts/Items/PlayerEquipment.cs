using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private List<WeaponData> _paladinWeapons;
    [SerializeField] private List<WeaponData> _barbarianWeapons;
    [SerializeField] private List<WeaponData> _warlockWeapons;

    private WeaponData _currentWeapon;

    public void Initialize(PlayerClassData playerClass, int indexWeapon)
    {
        if (playerClass.ClassName == TypePlayerClass.Paladin)
            _currentWeapon = _paladinWeapons[indexWeapon];

        if (playerClass.ClassName == TypePlayerClass.Barbarian)
            _currentWeapon = _barbarianWeapons[indexWeapon];

        if (playerClass.ClassName == TypePlayerClass.Warlock)
            _currentWeapon = _warlockWeapons[indexWeapon];
    }
}