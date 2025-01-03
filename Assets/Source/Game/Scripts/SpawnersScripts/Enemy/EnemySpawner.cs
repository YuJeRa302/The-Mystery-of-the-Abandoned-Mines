using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class EnemySpawner : IDisposable
    {
        private readonly System.Random _rnd = new ();
        private readonly float _delaySpawn = 1.0f;
        private readonly int _minValue = 0;

        private Pool _enemuPool;
        private Transform[] _spawnPoints;
        private Player _player;
        private EnemyData[] _enemyDatas;
        private Coroutine _spawn;
        private ICoroutineRunner _coroutineRunner;
        private List<Enemy> _enemies = new List<Enemy>();
        private int _currentWaveNumber = 0;
        private int _countWaves;
        private float _timeAfterLastSpawn;
        private float _delay = 5f;
        private int _waveLenght = 1;
        private int _spawnedEnemy;
        private int _deadEnemy = 0;
        private int _countEnemyRoom = 1;//временное значение
        private int _totalEnemyCount;

        public event Action AllEnemyRoomDied;

        public EnemySpawner (Pool enemyPool, ICoroutineRunner coroutineRunner)
        {
            _enemuPool = enemyPool;
            _coroutineRunner = coroutineRunner;
        }

        public void Dispose()
        {
            if (_spawn != null)
                _coroutineRunner.StopCoroutine(_spawn);

            GC.SuppressFinalize(this);
        }

        public void Initialize(RoomView currentRoom)
        {
            if (_spawn != null)
                _coroutineRunner.StopCoroutine(_spawn);

            if(currentRoom.EnemySpawnPoints.Length == 0)
            {
                AllEnemyRoomDied?.Invoke();
            }
            else
            {
                _spawnedEnemy = 0;
                _deadEnemy = 0;
                _countEnemyRoom = currentRoom.CountEnemy;
                _spawnPoints = currentRoom.EnemySpawnPoints;
                _enemyDatas = currentRoom.RoomData.EnemyData;
                _spawn = _coroutineRunner.StartCoroutine(Spawn());
            }
        }

        public void SetTotalEnemyCount(int countEnemy, Player player)
        {
            _player = player;
            _totalEnemyCount = countEnemy;
        }

        private IEnumerator Spawn()
        {
            //while (_maxGameTime > _minValue)
            //{
            //    yield return new WaitForSeconds(_delaySpawn);
            //    EnemyCreate(_enemyDatas[_rnd.Next(_enemyDatas.Length)], _rnd.Next(_spawnPoints.Length));
            //    _maxGameTime--;
            //}

            while (_spawnedEnemy < _countEnemyRoom)
            {
                yield return new WaitForSeconds(_delaySpawn);
                EnemyCreate(_enemyDatas[_rnd.Next(_enemyDatas.Length)], _rnd.Next(_spawnPoints.Length));
                _spawnedEnemy++;
            }
        }

        private void EnemyCreate(EnemyData enemyData, int value)
        {
            Enemy enemy = null;

            if (TryFindEnemy(enemyData.PrefabEnemy.gameObject, out Enemy poolEnemy))
            {
                enemy = poolEnemy;
                enemy.transform.position = _spawnPoints[value].position;
                enemy.gameObject.SetActive(true);
                enemy.ResetEnemy();
            }
            else
            {
                enemy = GameObject.Instantiate(enemyData.PrefabEnemy, _spawnPoints[value].position, _spawnPoints[value].rotation);

                _enemuPool.InstantiatePoolObject(enemy, enemyData.PrefabEnemy.name);
                enemy.Initialize(_player, enemyData.Id);
                enemy.Died += OnEnemyDead;
                _enemies.Add(enemy);
            }
        }

        private bool TryFindEnemy(GameObject enemyType, out Enemy poolEnemy)
        {
            poolEnemy = null;

            if (_enemuPool.TryPoolObject(enemyType, out PoolObject enemyPool))
            {
                poolEnemy = enemyPool as Enemy;
            }

            return poolEnemy != null;
        }

        private void OnEnemyDead()
        {
            _deadEnemy++;

            if (_deadEnemy == Convert.ToInt32(_countEnemyRoom))
            {
                AllEnemyRoomDied?.Invoke();
            }

            _totalEnemyCount--;

            if(_totalEnemyCount <= 0)
            {
                Debug.Log("!WIN!");
            }
        }
    }
}