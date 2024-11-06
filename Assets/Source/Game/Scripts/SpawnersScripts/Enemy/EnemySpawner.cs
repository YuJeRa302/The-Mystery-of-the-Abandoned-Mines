using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class EnemySpawner : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();
        private readonly float _delaySpawn = 5.0f;
        private readonly int _minValue = 0;

        [SerializeField] private Player _player;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private List<Enemy> _enemies;
        [SerializeField] private Pool _enemuPool;

        private EnemyData[] _enemyDatas;
        private IEnumerator _spawn;
        private int _currentWaveNumber = 0;
        private int _countWaves;
        private float _timeAfterLastSpawn;
        private float _delay = 5f;
        private int _waveLenght = 1;
        private int _spawnedEnemy;
        private int _deadEnemy = 0;
        private int _countStats;
        private float _maxGameTime = 2f;//временное значение

        public event Action AllEnemyRoomDied;

        //private void Update()
        //{
        //    _timeAfterLastSpawn += Time.deltaTime;

        //    if (_timeAfterLastSpawn >= _delay)
        //    {
        //        InitializeEnemy();
        //        _spawned++;
        //        _timeAfterLastSpawn = 0;
        //    }

        //}

        public void Initialize(Transform[] spawnPoints, EnemyData[] enemyDatas, int currentRoomLevel)
        {
            if (_spawn != null)
                StopCoroutine(_spawn);

            _spawnedEnemy = 0;
            _deadEnemy = 0;
            _spawnPoints = spawnPoints;
            _enemyDatas = enemyDatas;
            _spawn = Spawn();
            StartCoroutine(_spawn);
        }

        private IEnumerator Spawn()
        {
            //while (_maxGameTime > _minValue)
            //{
            //    yield return new WaitForSeconds(_delaySpawn);
            //    EnemyCreate(_enemyDatas[_rnd.Next(_enemyDatas.Length)], _rnd.Next(_spawnPoints.Length));
            //    _maxGameTime--;
            //}

            while (_spawnedEnemy < _maxGameTime)
            {
                yield return new WaitForSeconds(_delaySpawn);
                EnemyCreate(_enemyDatas[_rnd.Next(_enemyDatas.Length)], _rnd.Next(_spawnPoints.Length));
                _spawnedEnemy++;
            }
        }

        private void EnemyCreate(EnemyData enemyData, int value)
        {
            Enemy enemy = null;

            if (TyrFindEnemy(enemyData.PrefabEnemy, out Enemy poolEnemy))
            {
                enemy = poolEnemy;
                enemy.transform.position = _spawnPoints[value].position;
                enemy.gameObject.SetActive(true);
                enemy.ResetEnemy();
            }
            else
            {
                enemy = Instantiate(
                enemyData.PrefabEnemy,
                new Vector3(
                _spawnPoints[value].position.x,
                _spawnPoints[value].position.y,
                _spawnPoints[value].position.z),
                new Quaternion(_minValue, _minValue, _minValue, _minValue));

                enemy.Initialize(_player);
                enemy.Died += OnEnemyDead;
                _enemies.Add(enemy);
            }
        }

        private bool TyrFindEnemy(PoolObject enemyType, out Enemy poolEnemy)
        {
            Enemy enemy = enemyType as Enemy;
            poolEnemy = null;

            if (_enemuPool.TryPoolObject(out PoolObject enemyPool))
            {
                poolEnemy = enemyPool as Enemy;
            }

            return poolEnemy != null;
        }

        private void OnEnemyDead()
        {
            _deadEnemy++;

            if (_deadEnemy == Convert.ToInt32(_maxGameTime))
            {
                AllEnemyRoomDied?.Invoke();
            }
        }
    }
}