using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAttacker : IDisposable
    {
        private Transform _shotPoint;
        private ProjectileSpawner _bulletSpawner;
        private Pool _poolBullet;

        private float _damage;
        private float _searchRadius = 5f;//tests
        private float _attackDelay = 2f;
        private float _timeAfterLastAttack = 0f;
        private Player _player;
        private ICoroutineRunner _coroutineRunner;
        private Coroutine _findTarget;
        private Coroutine _coolDownAttack;
        private WeaponData _weaponData;
        private Enemy _currentTarget;
        private Dictionary<float, Enemy> _enemies = new Dictionary<float, Enemy>();

        public event Action Attacked;

        public PlayerAttacker(Transform shoPoint, Player player, WeaponData weaponData, ICoroutineRunner coroutineRunner, Pool pool)
        {
            _shotPoint = shoPoint;
            _player = player;
            _weaponData = weaponData;
            _coroutineRunner = coroutineRunner;
            _damage = _weaponData.BonusDamage;
            _poolBullet = pool;

            if (_weaponData.TargetClass == TypePlayerClass.Warlock)
            {
                _searchRadius = 25f;
                WarlockWeaponData paladinWeaponData = weaponData as WarlockWeaponData;
                _bulletSpawner = new ProjectileSpawner(paladinWeaponData.BulletPrafab, _poolBullet, _shotPoint, _damage);
            }

            _coolDownAttack = _coroutineRunner.StartCoroutine(CoolDownAttack());
        }

        public void Dispose()
        {
            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);

            if (_coolDownAttack != null)
                _coroutineRunner.StopCoroutine(_coolDownAttack);

            GC.SuppressFinalize(this);
        }

        public void AttackEnemy()
        {
            if (_weaponData.TargetClass == TypePlayerClass.Warlock)
                InstantiateBullet();
            else
                ApplyDamage();
        }

        private IEnumerator CoolDownAttack()
        {
            while (_timeAfterLastAttack <= _attackDelay)
            {
                _timeAfterLastAttack += Time.deltaTime;
                yield return null;
            }

            _timeAfterLastAttack = 0;

            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);

            _findTarget = _coroutineRunner.StartCoroutine(FindTarget());
            
        }

        private IEnumerator FindTarget()
        {
            _enemies.Clear();
            _currentTarget = null;

            while (_currentTarget == null)
            {
                var colliders = Physics.OverlapSphere(_player.transform.position, _searchRadius);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].TryGetComponent(out Enemy enemy))
                    {
                        float distanceToTarget = Vector3.Distance(enemy.transform.position, _player.transform.position);

                        if (distanceToTarget <= _searchRadius)
                            _enemies.Add(distanceToTarget, enemy);
                    }
                }

                if (_enemies.Count == 0)
                {
                    yield return null;
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
        }

        private void GetHit()
        {
            Attacked?.Invoke();

            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);
        }

        private void ApplyDamage()
        {
            if (_currentTarget != null)
                _currentTarget.TakeDamage(_damage);

            if (_coolDownAttack != null)
                _coroutineRunner.StopCoroutine(_coolDownAttack);

            _coolDownAttack = _coroutineRunner.StartCoroutine(CoolDownAttack());
        }

        private void InstantiateBullet()
        {
            if (_currentTarget != null)
                _bulletSpawner.SpawnProjectile(_currentTarget);

            if (_coolDownAttack != null)
                _coroutineRunner.StopCoroutine(_coolDownAttack);

            _coolDownAttack = _coroutineRunner.StartCoroutine(CoolDownAttack());
        }
    }
}