using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAttacker : IDisposable
    {
        private readonly System.Random _rnd = new();

        private Transform _shotPoint;
        private ProjectileSpawner _bulletSpawner;
        private Pool _poolBullet;

        private float _damage;
        private float _lastApplaedDamage;
        private float _searchRadius = 5f;//tests
        private float _attackRange = 5f;//tests
        private float _attackDelay = 2f;
        private float _timeAfterLastAttack = 0f;
        private Player _player;
        private ICoroutineRunner _coroutineRunner;
        private Coroutine _findTarget;
        private Coroutine _coolDownAttack;
        //private WeaponData _weaponData;
        private DamageParametr _damageParametr;
        private Enemy _currentTarget;
        private float _chenceCritDamage;
        private float _critDamageMultiplier;
        private float _chenceVampirism;
        private float _vampirismValue;
        private bool _isRangeAttack = false;
        private Dictionary<float, Enemy> _enemies = new Dictionary<float, Enemy>();

        public event Action Attacked;
        public event Action CritAttacked;
        public event Action<Transform> EnemyFinded;
        public event Action<float> HealedVampirism;

        public PlayerAttacker(Transform shoPoint, Player player, WeaponData weaponData, ICoroutineRunner coroutineRunner, Pool pool)
        {
            _shotPoint = shoPoint;
            _player = player;
            //_weaponData = weaponData;
            //_damageParametr = _weaponData.DamageParametrs[0];
            _coroutineRunner = coroutineRunner;
            _poolBullet = pool;

            List<DamageSupportivePatametr> damageSupportivePatametrs = new List<DamageSupportivePatametr>(weaponData.DamageParametr.DamageSupportivePatametrs);

            for (int i = 0; i < damageSupportivePatametrs.Count; i++)
            {
                damageSupportivePatametrs[i] = new DamageSupportivePatametr(weaponData.DamageParametr.DamageSupportivePatametrs[i].Value, 
                    weaponData.DamageParametr.DamageSupportivePatametrs[i].SupportivePatametr);
            }

            _damageParametr = new DamageParametr(weaponData.DamageParametr.TypeDamage, damageSupportivePatametrs, weaponData.DamageParametr.Particle);

            foreach (var parametr in weaponData.WeaponParameter.WeaponSupportivePatametrs)
            {
                if (parametr.SupportivePatametr == TypeWeaponSupportiveParametr.CritChence)
                {
                    _chenceCritDamage = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeWeaponSupportiveParametr.CritDamage)
                {
                    _critDamageMultiplier = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeWeaponSupportiveParametr.LifeStealChance)
                {
                    _chenceVampirism = parametr.Value;
                }
                else if (parametr.SupportivePatametr == TypeWeaponSupportiveParametr.LifeStealValue)
                {
                    _vampirismValue = parametr.Value;
                }
            }

            if (weaponData.TargetClass == TypePlayerClass.Warlock)
            {
                _isRangeAttack = true;
                _attackRange = 10f;
                _searchRadius = 10f;
                WarlockWeaponData paladinWeaponData = weaponData as WarlockWeaponData;
                _bulletSpawner = new ProjectileSpawner(paladinWeaponData.BulletPrafab, _poolBullet, _shotPoint, _damage, _damageParametr);
            }

            foreach (var parametr in _damageParametr.DamageSupportivePatametrs)
            {
                if (parametr.SupportivePatametr == TypeSupportivePatametr.Damage)
                {
                    _damage = parametr.Value;
                }
            }

            _coolDownAttack = _coroutineRunner.StartCoroutine(CoolDownAttack());
        }

        public float Damage => _damage;
        public Enemy CurrentTarget => _currentTarget;
        public DamageParametr  DamageParametr => _damageParametr;

        public void Dispose()
        {
            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);

            if (_coolDownAttack != null)
                _coroutineRunner.StopCoroutine(_coolDownAttack);

            GC.SuppressFinalize(this);
        }

        public void ÑhangeDamage(float value)
        {
            _damage = value;

            foreach (var parametr in _damageParametr.DamageSupportivePatametrs)
            {
                if (parametr.SupportivePatametr == TypeSupportivePatametr.Damage)
                {
                    parametr.ChaneValue(_damage);
                }
            }
        }

        public void AttackEnemy()
        {
            if (_isRangeAttack)
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
                            if(_enemies.ContainsKey(distanceToTarget) == false)
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
                        EnemyFinded?.Invoke(_currentTarget.transform);
                        GetHit();
                    }
                }
            }
        }

        private void GetHit()
        {
            float distanceToTarget = Vector3.Distance(_currentTarget.transform.position, _player.transform.position);

            if (distanceToTarget <= _attackRange)
            {
                Attacked?.Invoke();
            }

            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);
        }

        private void ApplyDamage()
        {
            Vector3 directionToTarget = _player.transform.position - _currentTarget.transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            float critDamage;

            if (_currentTarget != null && distanceToTarget <= _attackRange)
            {
                foreach (var parametr in _damageParametr.DamageSupportivePatametrs)
                {
                    if (parametr.SupportivePatametr == TypeSupportivePatametr.Damage)
                    {
                        if (CalculatedChence(_chenceCritDamage))
                        {
                            _damage = parametr.Value;
                            critDamage = parametr.Value *(1 + _critDamageMultiplier / 100);
                            parametr.ChaneValue(critDamage);
                            _currentTarget.TakeDamageTest(_damageParametr);
                            _lastApplaedDamage = parametr.Value;
                            parametr.ChaneValue(_damage);
                            CritAttacked?.Invoke();
                        }
                        else
                        {
                            _currentTarget.TakeDamageTest(_damageParametr);
                            _lastApplaedDamage = parametr.Value;
                        }
                    }
                }

                if (CalculatedChence(_chenceVampirism))
                {
                    float healing = _lastApplaedDamage * (_vampirismValue / 100);
                    HealedVampirism?.Invoke(healing);
                }
            }

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

        private bool CalculatedChence(float chence)
        {
            if (_rnd.Next(1, 100) <= chence)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}