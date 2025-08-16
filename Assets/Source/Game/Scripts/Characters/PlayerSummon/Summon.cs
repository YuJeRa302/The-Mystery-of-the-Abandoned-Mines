using Assets.Source.Game.Scripts.PoolSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class Summon : PoolObject
    {
        private readonly float _distanceToTarget = 2f;
        private readonly float _searchRadius = 10f;

        [SerializeField] private SummonStateMachineExample _machineExample;
        [SerializeField] private SummonAnimation _animation;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private DamageSource _damageSource;

        private Player _player;
        private Enemy _target;
        private Coroutine _coroutine;
        private float _lifeTime;
        private float _attackDelay = 2f;
        private Dictionary<float, Enemy> _enemies = new Dictionary<float, Enemy>();
        private Collider[] _foundColliders = new Collider[50];

        public Enemy Target => _target;
        public SummonAnimation Animation => _animation;
        public float DistanceToTarget => _distanceToTarget;
        public float AttackDelay => _attackDelay;
        public DamageSource DamageSource => _damageSource;
        public float MoveSpeed => _moveSpeed;

        public void Initialize(Player player, DamageSource damageSource, float lifeTime)
        {
            _player = player;
            _damageSource = damageSource;
            _lifeTime = lifeTime;
            _machineExample.MachineInitialized += OnStateMashineInitialize;
            _machineExample.InitializeStateMashine(_player);

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(CountLifeTime());
        }

        public void SetTarget(Enemy target)
        {
            _target = target;
        }

        public bool FindEnemy(out Enemy target)
        {
            _enemies.Clear();
            var colliders = Physics.OverlapSphereNonAlloc(
                transform.position,
                _searchRadius,
                _foundColliders
            );

            for (int i = 0; i < colliders; i++)
            {
                if (_foundColliders[i].TryGetComponent(out Enemy enemy))
                {
                    float distanceToTarget = Vector3.Distance(enemy.transform.position, transform.position);

                    if (distanceToTarget <= _searchRadius)
                        if (_enemies.ContainsKey(distanceToTarget) == false)
                            _enemies.Add(distanceToTarget, enemy);
                }
            }

            if (_enemies.Count == 0)
            {
                target = null;
            }
            else
            {
                target = _enemies.OrderBy(distance => distance.Key).First().Value;

                if (target != null && target.isActiveAndEnabled == true)
                {
                    return true;
                }
            }

            return target != null && target.isActiveAndEnabled == true;
        }

        public void DisableTarget()
        {
            _target = null;
        }

        public void ResetStats()
        {
            _machineExample.ResetState();

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(CountLifeTime());
        }

        private void OnStateMashineInitialize()
        {
            _animation.Initialization(_machineExample);
        }

        private IEnumerator CountLifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            ReturnToPool();
        }
    }
}