using UnityEngine;

[CreateAssetMenu(fileName = "New Wapon", menuName = "Create Weapon/Paladin Weapon", order = 51)]
public class PaladinWeaponData : WeaponData
{
    [SerializeField] private WeaponView _additionalWeapon;

    public WeaponView AdditionalWeapon => _additionalWeapon;
}