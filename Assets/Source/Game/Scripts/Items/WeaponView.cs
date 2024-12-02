using Assets.Source.Game.Scripts;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    private Player _player;
    private WeaponData _weapon;

    public WeaponData WeaponData => _weapon;

    public void Initialize(Player player, WeaponData weaponData)
    {
        _player = player;
        _weapon = weaponData;
        Instantiate(_weapon.WeaponPrefab, _player.WeaponPoint);

        if (_weapon.TargetClass == TypePlayerClass.Paladin)
        {
            PaladinWeaponData paladinWeaponData = weaponData as PaladinWeaponData;
            Instantiate(paladinWeaponData.AdditionalWeapon, _player.AdditionalWeaponPoint);
        }
    }
}