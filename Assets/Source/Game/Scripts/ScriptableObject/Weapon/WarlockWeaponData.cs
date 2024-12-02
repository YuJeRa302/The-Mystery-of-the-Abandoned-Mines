using UnityEngine;

[CreateAssetMenu(fileName = "New Barbarian Wapon", menuName = "Create Weapon/Warlock Weapon", order = 51)]
public class WarlockWeaponData : WeaponData
{
    [SerializeField] private PlayerProjectile _bulletPrefab;

    public PlayerProjectile BulletPrafab => _bulletPrefab;
}