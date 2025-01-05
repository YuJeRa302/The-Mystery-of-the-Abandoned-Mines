using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class PlayerWeapons : IDisposable
{
    private Player _player;
    private WeaponData _weapon;
    private WeponPrefab _weponPrefab;

    public PlayerWeapons(Player player, WeaponData weaponData)
    {
        _player = player;
        _weapon = weaponData;
        _weponPrefab = _weapon.WeaponPrefab;

        CreateWeaponView();
    }

    public WeaponData WeaponData => _weapon;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void ChangeTrailEffect()
    {
        if(_weponPrefab.KickEffect != null)
            GameObject.Instantiate(_weponPrefab.KickEffect, _player.transform);
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