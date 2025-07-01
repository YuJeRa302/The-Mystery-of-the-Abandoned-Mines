using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class PlayerWeapons : IDisposable
{
    private Player _player;
    private WeaponData _weapon;
    private WeponPrefab _weponPrefab;
    private Pool _pool;
    private PoolParticle _critParticle;
    private PoolParticle _vampirismParticle;

    public PlayerWeapons(Player player, WeaponData weaponData, Pool pool, PoolParticle critParticle, PoolParticle vampiticmParticle)
    {
        _player = player;
        _weapon = weaponData;
        _weponPrefab = _weapon.WeaponPrefab;
        _pool = pool;
        _critParticle = critParticle;
        _vampirismParticle = vampiticmParticle;

        CreateWeaponView();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public WeaponData GetWeaponData() 
    {
        return _weapon;
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

    public void InstantiateCritEffect()
    {
        PoolParticle particle;

        if (_pool.TryPoolObject(_critParticle.gameObject, out PoolObject pollParticle))
        {
            particle = pollParticle as PoolParticle;
            particle.transform.position = _player.transform.position;
            particle.transform.rotation = _player.transform.rotation;
            particle.gameObject.SetActive(true);
        }
        else
        {
            particle = GameObject.Instantiate(_critParticle, _player.transform.position, _player.transform.rotation);
            _pool.InstantiatePoolObject(particle, _vampirismParticle.name);
        }
    }

    public void InstantiateHealingEffect()
    {
        PoolParticle particle;

        if (_pool.TryPoolObject(_vampirismParticle.gameObject, out PoolObject pollParticle))
        {
            particle = pollParticle as PoolParticle;
            particle.transform.position = _player.transform.position;
            particle.gameObject.SetActive(true);
        }
        else
        {
            particle = GameObject.Instantiate(_vampirismParticle, _player.transform);
            _pool.InstantiatePoolObject(particle, _critParticle.name);
        }
    }

    private void CreateWeaponView()
    {
        GameObject.Instantiate(_weapon.WeaponPrefab, _player.WeaponPoint);

        if (_weapon.TypePlayerClass == TypePlayerClass.Paladin)
        {
            PaladinWeaponData paladinWeaponData = _weapon as PaladinWeaponData;
            GameObject.Instantiate(paladinWeaponData.AdditionalWeapon, _player.AdditionalWeaponPoint);
        }
    }
}