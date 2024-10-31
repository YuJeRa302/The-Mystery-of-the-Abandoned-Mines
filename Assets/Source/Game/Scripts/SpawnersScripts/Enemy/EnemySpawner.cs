using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Player _player;
    [SerializeField] private List<Enemy> _enemies;

    private int _currentWaveNumber = 0;
    private int _countWaves;
    private float _timeAfterLastSpawn;
    private float _delay = 5f;
    private int _waveLenght = 1;
    private int _spawned;
    private int _countStats;

    private void Update()
    {
        _timeAfterLastSpawn += Time.deltaTime;

        if (_timeAfterLastSpawn >= _delay)
        {
            InitializeEnemy();
            _spawned++;
            _timeAfterLastSpawn = 0;
        }

    }

    private void InitializeEnemy()
    {
        int currentSpawnPont = UnityEngine.Random.Range(0, _spawnPoints.Length);
        Enemy enemy = _enemies[UnityEngine.Random.Range(0, _enemies.Count)];
        enemy = Instantiate(enemy, _spawnPoints[currentSpawnPont].position, _spawnPoints[currentSpawnPont].rotation, _spawnPoints[currentSpawnPont]).GetComponent<Enemy>();
        enemy.Initialize(_player);
    }
}