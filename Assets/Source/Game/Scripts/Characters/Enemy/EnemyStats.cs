using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    [SerializeField] private int _damage;
    [SerializeField] private int _health;
    [SerializeField] private int _experienceReward;
    [SerializeField] private int _upgradeExperienceReward;
    [SerializeField] private int _score;
    [SerializeField] private float _delaySpawn;
    [SerializeField] private int _enemyCount;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackDelay;
    [SerializeField] private int _chanceSpawn;
    [SerializeField] private int _goldReward;

    public int Damage => _damage;
    public int Health => _health;
    public int ExperienceReward => _experienceReward;
    public int UpgradeExperienceReward => _upgradeExperienceReward;
    public int Score => _score;
    public float DelaySpawn => _delaySpawn;
    public int EnemyCount => _enemyCount;
    public float MoveSpeed => _moveSpeed;
    public float AttackDistance => _attackDistance;
    public float AttackDelay => _attackDelay;
    public int ChanceSpawn => _chanceSpawn;
    public int GoldReward => _goldReward;
}