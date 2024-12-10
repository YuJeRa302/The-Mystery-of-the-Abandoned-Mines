using UnityEngine;

[CreateAssetMenu(fileName = "New Wapon", menuName = "Create Weapon/Paladin Weapon", order = 51)]
public class PaladinWeaponData : WeaponData
{
    [SerializeField] private WeponPrefab _additionalWeapon;

    public WeponPrefab AdditionalWeapon => _additionalWeapon;
}