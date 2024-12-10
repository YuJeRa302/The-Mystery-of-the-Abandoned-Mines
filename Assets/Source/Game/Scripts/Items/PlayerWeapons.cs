using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class PlayerWeapons : IDisposable
{
    private Player _player;
    private WeaponData _weapon;

    public PlayerWeapons(Player player, WeaponData weaponData)
    {
        _player = player;
        _weapon = weaponData;

        CreateWeaponView();
    }

    public WeaponData WeaponData => _weapon;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void CreateWeaponView()
    {
        GameObject.Instantiate(_weapon.WeaponPrefab, _player.WeaponPoint);

        if (_weapon.TargetClass == TypePlayerClass.Paladin)
        {
            PaladinWeaponData paladinWeaponData = _weapon as PaladinWeaponData;
            GameObject.Instantiate(paladinWeaponData.AdditionalWeapon, _player.AdditionalWeaponPoint);
        }
    }
}