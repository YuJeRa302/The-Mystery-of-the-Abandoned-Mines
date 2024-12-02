using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAttacker : MonoBehaviour
    {
        private float SearchRadius = 5f;//tests

        private float _damage;
        [SerializeField] private Transform _shotPoint;
        [SerializeField] private ProjectileSpawner _bulletSpawner;
        [SerializeField] private Pool _poolBullet;

        private float _attackDelay = 2f;
        private float _timeAfterLastAttack = 0f;
        private WeaponData _weaponData;
        private Enemy _currentTarget;
        private Dictionary<float, Enemy> _enemies = new Dictionary<float, Enemy>();

        public event Action Attacked;

        private void Update()
        {
            _timeAfterLastAttack += Time.deltaTime;

            if (_timeAfterLastAttack >= _attackDelay)
            {
                FindTarget();
                _timeAfterLastAttack = 0;
            }
        }

        public void Initialize(WeaponData weapon)
        {
            _weaponData = weapon;
            _damage = _weaponData.BonusDamage;

            if (_weaponData.TargetClass == TypePlayerClass.Warlock)
            {
                SearchRadius = 25f;
                WarlockWeaponData paladinWeaponData = weapon as WarlockWeaponData;
                _bulletSpawner.Initialize(paladinWeaponData.BulletPrafab, _poolBullet, _shotPoint, _damage);
            }
        }

        private void FindTarget()
        {
            _enemies.Clear();
            _currentTarget = null;

            var colliders = Physics.OverlapSphere(transform.position, SearchRadius);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent(out Enemy enemy))
                {
                    float distanceToTarget = Vector3.Distance(enemy.transform.position, transform.position);

                    if (distanceToTarget <= SearchRadius)
                        _enemies.Add(distanceToTarget, enemy);
                }
            }

            if (_enemies.Count == 0)
            {
                return;
            }
            else
            {
                _currentTarget = _enemies.OrderBy(distance => distance.Key).First().Value;

                if (_currentTarget != null && _currentTarget.isActiveAndEnabled == true)
                {
                    GetHit();
                }
            }
        }

        private void GetHit()
        {
            Attacked?.Invoke();
        }

        private void ApplyDamage()
        {
            if(_currentTarget != null)
                _currentTarget.TakeDamage(_damage);
        }

        private void InstantiateBullet()
        {
            if (_currentTarget != null)
                _bulletSpawner.SpawnProjectile(_currentTarget);
        }
    }
}