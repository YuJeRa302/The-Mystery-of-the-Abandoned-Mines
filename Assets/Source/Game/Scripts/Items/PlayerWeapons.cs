using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class PlayerWeapons : IDisposable
{
    private Player _player;
    private WeaponData _weapon;
    private WeponPrefab _weponPrefab;
    private Pool _pool;

    public PlayerWeapons(Player player, WeaponData weaponData, Pool pool)
    {
        _player = player;
        _weapon = weaponData;
        _weponPrefab = _weapon.WeaponPrefab;
        _pool = pool;

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
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(_weponPrefab.KickEffect.gameObject, out PoolObject pollParticle))
            {
                particle = pollParticle as PoolParticle;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = GameObject.Instantiate(_weponPrefab.KickEffect, _player.transform);
                _pool.InstantiatePoolObject(particle, _weponPrefab.KickEffect.name);
            }
        }
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