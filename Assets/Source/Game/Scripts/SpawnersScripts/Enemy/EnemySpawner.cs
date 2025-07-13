using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Menu;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.SpawnersScripts
{
    public class EnemySpawner : IDisposable
    {
        private readonly System.Random _rnd = new();
        private readonly int _minValueChance = 0;
        private readonly int _maxValueChance = 100;

        private int _currentRoomsLevel = 0;
        private Pool _enemuPool;
        private Transform[] _spawnPoints;
        private Player _player;
        private Coroutine _spawn;
        private Coroutine _spawnEnemy;
        private ICoroutineRunner _coroutineRunner;
        private int _currentRoomLevel;
        private List<Enemy> _enemies = new();
        private Dictionary<string, PoolParticle> _deadParticles = new Dictionary<string, PoolParticle>();
        private int _deadEnemy = 0;
        private int _totalEnemyCount;
        private int _currentEnemyCount;
        private bool _isEnemySpawned = false;
        private AudioPlayer _audioPlayer;
        private int _currentLevelTier;

        public EnemySpawner(Pool enemyPool,
            ICoroutineRunner coroutineRunner,
            AudioPlayer audioPlayer,
            int currentLevelTier)
        {
            _enemuPool = enemyPool;
            _coroutineRunner = coroutineRunner;
            _audioPlayer = audioPlayer;
            _currentLevelTier = currentLevelTier;
        }

        public event Action<Enemy> EnemyDied;
        public event Action AllEnemyRoomDied;

        public void Dispose()
        {
            if (_spawn != null)
                _coroutineRunner.StopCoroutine(_spawn);

            if (_spawnEnemy != null)
                _coroutineRunner.StopCoroutine(_spawnEnemy);

            if (_player != null)
                _player.PlayerDied -= OnPlayerDead;

            foreach (Enemy enemy in _enemies)
            {
                enemy.Died -= OnEnemyDead;
                enemy.PlayerAttacked -= OnEnemyAttack;
            }

            GC.SuppressFinalize(this);
        }

        public void InitPlayerInstance(Player player)
        {
            _player = player;
            _player.PlayerDied += OnPlayerDead;
        }

        public void EnterRoom(RoomView currentRoom)
        {
            if (currentRoom.EnemySpawnPoints.Length == 0)
                return;

            if (_spawn != null)
                _coroutineRunner.StopCoroutine(_spawn);

            if (_spawnEnemy != null)
                _coroutineRunner.StopCoroutine(_spawnEnemy);

            _deadEnemy = 0;
            _currentEnemyCount = 0;
            _currentRoomLevel = currentRoom.CurrentLevel;
            _spawnPoints = currentRoom.EnemySpawnPoints;
            _totalEnemyCount = GetTotalEnemyCount(currentRoom.RoomData.EnemyDatas);
            _spawn = _coroutineRunner.StartCoroutine(Spawn(currentRoom.RoomData.EnemyDatas));
        }

        public void ClearEnemyInRoom()
        {
            if (_spawn != null)
                _coroutineRunner.StopCoroutine(_spawn);

            if (_spawnEnemy != null)
                _coroutineRunner.StopCoroutine(_spawnEnemy);

            foreach (Enemy enemy in _enemies)
            {
                if (enemy.gameObject.activeSelf)
                    enemy.ReturnObjectPool();
            }
        }

        private int GetTotalEnemyCount(EnemyData[] enemyDatas)
        {
            int countEnemy = 0;

            foreach (EnemyData enemyData in enemyDatas)
            {
                countEnemy += enemyData.EnemyStats[_currentLevelTier].EnemyCount;
            }

            return countEnemy;
        }

        private IEnumerator Spawn(EnemyData[] enemyDatas)
        {
            foreach (EnemyData enemyData in enemyDatas)
            {
                if (_spawnEnemy != null)
                    _coroutineRunner.StopCoroutine(_spawnEnemy);

                _isEnemySpawned = false;
                _spawnEnemy = _coroutineRunner.StartCoroutine(SpawnEnemy(enemyData));
                yield return new WaitUntil(() => _isEnemySpawned);
            }
        }

        private IEnumerator SpawnEnemy(EnemyData enemyData)
        {
            if (CalculateChance(enemyData.EnemyStats[_currentLevelTier].ChanceSpawn))
            {
                int currentEnemyCount = 0;

                if (enemyData.PrefabEnemy is Boss)
                {
                    if (_currentEnemyCount > 0)
                    {
                        yield return null;
                        _totalEnemyCount--;
                        _isEnemySpawned = true;
                    }
                }

                if (_currentEnemyCount < _totalEnemyCount)
                {
                    while (currentEnemyCount < enemyData.EnemyStats[_currentLevelTier].EnemyCount)
                    {
                        yield return new WaitForSeconds(enemyData.EnemyStats[_currentLevelTier].DelaySpawn);
                        CreateEnemy(enemyData, _rnd.Next(_spawnPoints.Length));
                        currentEnemyCount++;
                        _currentEnemyCount++;
                    }
                }

                _isEnemySpawned = true;
            }
            else
            {
                yield return null;
                _totalEnemyCount -= enemyData.EnemyStats[_currentLevelTier].EnemyCount;
                _isEnemySpawned = true;
            }
        }

        private void CreateEnemy(EnemyData enemyData, int value)
        {
            Enemy enemy = null;

            if (TryFindEnemy(enemyData.PrefabEnemy.gameObject, out Enemy poolEnemy))
            {
                enemy = poolEnemy;
                enemy.transform.position = _spawnPoints[value].position;
                enemy.gameObject.SetActive(true);
                enemy.ResetEnemy(_currentRoomLevel);
            }
            else
            {
                enemy = UnityEngine.Object.Instantiate(enemyData.PrefabEnemy,
                    _spawnPoints[value].position, _spawnPoints[value].rotation);
                _enemuPool.InstantiatePoolObject(enemy, enemyData.PrefabEnemy.name);
                enemy.Initialize(_player, _currentRoomLevel, enemyData, _currentLevelTier);
                enemy.Died += OnEnemyDead;
                enemy.PlayerAttacked += OnEnemyAttack;
                _enemies.Add(enemy);

                if (_deadParticles.ContainsKey(enemyData.PrefabEnemy.name) == false)
                    _deadParticles.Add(enemyData.PrefabEnemy.name, enemyData.EnemyDieParticleSystem);
            }
        }

        private void OnEnemyAttack(AudioClip audioClip)
        {
            _audioPlayer.PlayCharacterAudio(audioClip);
        }

        private bool TryFindEnemy(GameObject enemyType, out Enemy poolEnemy)
        {
            poolEnemy = null;

            if (_enemuPool.TryPoolObject(enemyType, out PoolObject enemyPool))
                poolEnemy = enemyPool as Enemy;

            return poolEnemy != null;
        }

        private void OnEnemyDead(Enemy enemy)
        {
            _audioPlayer.PlayCharacterAudio(enemy.DeathAudio);
            EnemyDied?.Invoke(enemy);

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
                    particle = UnityEngine.Object.Instantiate(particlePrefab,
                        enemy.transform.position, Quaternion.identity);
                    _enemuPool.InstantiatePoolObject(particle, particlePrefab.name);
                }
            }

            EnemyDeadCounter();
        }

        private void EnemyDeadCounter()
        {
            _deadEnemy++;

            if (_deadEnemy == _totalEnemyCount)
            {
                AllEnemyRoomDied?.Invoke();
            }
        }

        private bool CalculateChance(float chance)
        {
            if (_rnd.Next(_minValueChance, _maxValueChance) <= chance + _currentRoomsLevel)
                return true;
            else
                return false;
        }

        private void OnPlayerDead()
        {
            if (_spawn != null)
                _coroutineRunner.StopCoroutine(_spawn);

            if (_spawnEnemy != null)
                _coroutineRunner.StopCoroutine(_spawnEnemy);

            foreach (Enemy enemy in _enemies)
            {
                if (enemy.gameObject.activeSelf)
                    enemy.ReturnObjectPool();
            }
        }
    }
}