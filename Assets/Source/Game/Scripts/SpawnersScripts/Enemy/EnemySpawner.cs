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

        private Pool _enemuPool;
        private Transform[] _spawnPoints;
        private Player _player;
        private EnemyData[] _enemyDatas;
        private EnemyData[] _epicEnemyDatas;
        private Coroutine _spawn;
        private ICoroutineRunner _coroutineRunner;
        private RoomView _currentRoom;
        private List<Enemy> _enemies = new List<Enemy>();
        private Dictionary<string, PoolParticle> _deadParticles = new Dictionary<string, PoolParticle>();
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
            _currentRoom = currentRoom;

            if (_spawn != null)
                _coroutineRunner.StopCoroutine(_spawn);

            //if (_deadParticles != null)
            //    _deadParticles.Clear();

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
                _epicEnemyDatas = currentRoom.RoomData.EpicEnemyDatas;
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
            while (_spawnedEnemy < _countEnemyRoom)
            {
                yield return new WaitForSeconds(_delaySpawn);
                
                if (_rnd.Next(0, 100) <= 90)
                {
                    EnemyCreate(_epicEnemyDatas[_rnd.Next(_epicEnemyDatas.Length)], _rnd.Next(_spawnPoints.Length));
                }
                else
                {
                    EnemyCreate(_enemyDatas[_rnd.Next(_enemyDatas.Length)], _rnd.Next(_spawnPoints.Length));
                }

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
                enemy.ResetEnemy(_currentRoom.CurrentLevel);
            }
            else
            {
                enemy = GameObject.Instantiate(enemyData.PrefabEnemy, _spawnPoints[value].position, _spawnPoints[value].rotation);

                _enemuPool.InstantiatePoolObject(enemy, enemyData.PrefabEnemy.name);
                enemy.Initialize(_player, enemyData.Id, _currentRoom.CurrentLevel, enemyData.Damage, enemyData.Health, enemyData.AttackDelay, enemyData.AttackDistance, enemyData.MoveSpeed);
                enemy.Died += OnEnemyDead;
                _enemies.Add(enemy);

                if (_deadParticles.ContainsKey(enemyData.PrefabEnemy.name) == false)
                {
                    _deadParticles.Add(enemyData.PrefabEnemy.name, enemyData.EnemyDieParticleSystem);
                }
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

        private void OnEnemyDead(Enemy enemy)
        {
            if (_deadParticles.TryGetValue(enemy.NameObject, out PoolParticle particlePrefab))
            {
                PoolParticle particle;

                if (_enemuPool.TryPoolObject(particlePrefab.gameObject, out PoolObject pollParticle))
                {
                    particle = pollParticle as PoolParticle;
                    particle.transform.position = enemy.transform.position;
                    particle.gameObject.SetActive(true);
                }
                else
                {
                    particle = GameObject.Instantiate(particlePrefab, enemy.transform.position, Quaternion.identity);
                    _enemuPool.InstantiatePoolObject(particle, particlePrefab.name);
                }
            }

            EnemyDeadCounter();
        }

        private void EnemyDeadCounter()
        {
            _deadEnemy++;

            if (_deadEnemy == Convert.ToInt32(_countEnemyRoom))
            {
                AllEnemyRoomDied?.Invoke();
            }

            _totalEnemyCount--;

            if (_totalEnemyCount <= 0)
            {
                Debug.Log("!WIN!");
            }
        }
    }
}