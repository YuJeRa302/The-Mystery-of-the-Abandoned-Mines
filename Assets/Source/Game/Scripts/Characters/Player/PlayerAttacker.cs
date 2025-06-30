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
        private readonly IGameLoopService _gameLoopService;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly int _divider = 100;
        private readonly int _defaultCriticalDamageMultiplier = 1;
        private readonly int _minValueChance = 1;
        private readonly int _maxValueChance = 100;

        private Transform _shotPoint;
        private ProjectileSpawner _bulletSpawner;
        private Pool _poolBullet;
        private float _attackDelay = 2f;
        private float _timeAfterLastAttack = 0f;
        private Player _player;
        private Coroutine _findTarget;
        private Coroutine _coolDownAttack;
        private Enemy _currentTarget;
        private Dictionary<float, Enemy> _enemies = new Dictionary<float, Enemy>();
        private TypeAttackRange _typeAttackRange;

        public event Action Attacked;
        public event Action CritAttacked;
        public event Action<Transform> EnemyFinded;
        public event Action<float> HealedVampirism;

        public PlayerAttacker(
            Transform shoPoint,
            Player player,
            TypeAttackRange typeAttackRange,
            WeaponData weaponData,
            ICoroutineRunner coroutineRunner,
            IGameLoopService gameLoopService,
            Pool pool)
        {
            _gameLoopService = gameLoopService;
            _coroutineRunner = coroutineRunner;
            _shotPoint = shoPoint;
            _player = player;
            _typeAttackRange = typeAttackRange;
            _poolBullet = pool;
            CreateBulletSpawner(weaponData);
            AddListeners();
            _coolDownAttack = _coroutineRunner.StartCoroutine(CoolDownAttack());
        }

        public void Dispose()
        {
            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);

            if (_coolDownAttack != null)
                _coroutineRunner.StopCoroutine(_coolDownAttack);

            RemoveListeners();
            GC.SuppressFinalize(this);
        }

        public void AttackEnemy()
        {
            if (_typeAttackRange == TypeAttackRange.Ranged)
                InstantiateBullet();
            else
                ApplyDamage();
        }

        private void CreateBulletSpawner(WeaponData weaponData)
        {
            if (_typeAttackRange == TypeAttackRange.Ranged)
                _bulletSpawner = new ProjectileSpawner((weaponData as WarlockWeaponData).BulletPrafab, _poolBullet, _shotPoint, _player.DamageSource);
        }

        private void AddListeners()
        {
            _gameLoopService.GamePaused += OnGamePaused;
            _gameLoopService.GameResumed += OnGameResumed;
        }

        private void RemoveListeners()
        {
            _gameLoopService.GamePaused -= OnGamePaused;
            _gameLoopService.GameResumed -= OnGameResumed;
        }

        private void OnGamePaused(bool state)
        {
            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);

            if (_coolDownAttack != null)
                _coroutineRunner.StopCoroutine(_coolDownAttack);
        }

        private void OnGameResumed(bool state)
        {
            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);

            if (_coolDownAttack != null)
                _coroutineRunner.StopCoroutine(_coolDownAttack);

            _findTarget = _coroutineRunner.StartCoroutine(FindTarget());
            _coolDownAttack = _coroutineRunner.StartCoroutine(CoolDownAttack());
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
                var colliders = Physics.OverlapSphere(_player.transform.position, _player.SearchRadius);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].TryGetComponent(out Enemy enemy))
                    {
                        float distanceToTarget = Vector3.Distance(enemy.transform.position, _player.transform.position);

                        if (distanceToTarget <= _player.SearchRadius)
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

            if (distanceToTarget <= _player.AttackRange)
                Attacked?.Invoke();

            if (_findTarget != null)
                _coroutineRunner.StopCoroutine(_findTarget);
        }

        private void ApplyDamage()
        {
            if (_currentTarget == null)
                return;

            Vector3 directionToTarget = _player.transform.position - _currentTarget.transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= _player.AttackRange)
            {
                if (CalculateChance(_player.ChanceCriticalDamage))
                {
                    DamageSource criticalDamageSource = new(
                        _player.DamageSource.TypeDamage,
                        _player.DamageSource.DamageParameters,
                        _player.DamageSource.PoolParticle,
                        _player.DamageSource.Damage * (_defaultCriticalDamageMultiplier + _player.CriticalDamageMultiplier / _divider));

                    _currentTarget.TakeDamage(criticalDamageSource);
                    CritAttacked?.Invoke();
                }
                else
                {
                    _currentTarget.TakeDamage(_player.DamageSource);
                }

                if (CalculateChance(_player.ChanceVampirism))
                    HealedVampirism?.Invoke(_player.DamageSource.Damage * (_player.VampirismValue / _divider));
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

        private bool CalculateChance(float chance)
        {
            if (_rnd.Next(_minValueChance, _maxValueChance) <= chance)
                return true;
            else
                return false;
        }
    }
}