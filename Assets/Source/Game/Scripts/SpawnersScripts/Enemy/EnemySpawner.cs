using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class EnemySpawner : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();
        private readonly float _delaySpawn = 1.0f;
        private readonly int _minValue = 0;

        [SerializeField] private Player _player;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private List<Enemy> _enemies;

        private EnemyData[] _enemyDatas;
        private IEnumerator _spawn;
        private int _currentWaveNumber = 0;
        private int _countWaves;
        private float _timeAfterLastSpawn;
        private float _delay = 5f;
        private int _waveLenght = 1;
        private int _spawned;
        private int _countStats;
        private float _maxGameTime = 120;//временное значение

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
            _spawnPoints = spawnPoints;
            _enemyDatas = enemyDatas;
            _spawn = Spawn();
            StartCoroutine(_spawn);
        }

        private IEnumerator Spawn()
        {
            while (_maxGameTime > _minValue)
            {
                yield return new WaitForSeconds(_delaySpawn);
                EnemyCreate(_enemyDatas[_rnd.Next(_enemyDatas.Length)], _rnd.Next(_spawnPoints.Length));
                _maxGameTime--;
            }
        }

        private void EnemyCreate(EnemyData enemyData, int value)
        {
            Enemy enemy = Instantiate(
                enemyData.PrefabEnemy,
                new Vector3(
                _spawnPoints[value].position.x,
                _spawnPoints[value].position.y,
                _spawnPoints[value].position.z),
                new Quaternion(_minValue, _minValue, _minValue, _minValue));

            enemy.Initialize(_player);
            _enemies.Add(enemy);
        }

        //private void InitializeEnemy()
        //{
        //    int currentSpawnPont = _rnd.Next(0, _spawnPoints.Length);
        //    Enemy enemy = _enemies[_rnd.Next(0, _enemies.Count)];
        //    enemy = Instantiate(enemy, _spawnPoints[currentSpawnPont].position, _spawnPoints[currentSpawnPont].rotation, _spawnPoints[currentSpawnPont]).GetComponent<Enemy>();
        //    enemy.Initialize(_player);
        //}
    }
}