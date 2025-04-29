using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Create Enemy", order = 51)]
    public class EnemyData : ScriptableObject
    {
        [Header("[Enemy Stats]")]
        [SerializeField] private int _id;
        [SerializeField] private int _level;
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
        [Header("[Ability Stats]")]
        [SerializeField] private float[] _abilityCoolDown;
        [SerializeField] private int[] _abilityDamage;
        [Header("[ParticleSystem]")]
        //[SerializeField] private ParticleSystem _enemyHitParticleSystem;
        [SerializeField] private PoolParticle _enemyDieParticleSystem;
        //[SerializeField] private ParticleSystem _enemyAbilityParticleSystem;
        [Header("[Enemy Sound]")]
        [SerializeField] private AudioClip _audioClipDie;
        [SerializeField] private AudioClip _hit;
        [Header("[Enemy]")]
        [SerializeField] private Enemy _prefabEnemy;
        [SerializeField] private TypeEnemy _type;

        public int Id => _id;
        public int UpgradeExperienceReward => _upgradeExperienceReward;
        public int EnemyCount => _enemyCount;
        public float DelaySpawn => _delaySpawn;
        public int Level => _level;
        public int ChanceSpawn => _chanceSpawn;
        public int Damage => _damage;
        public int Health => _health;
        public int ExperienceReward => _experienceReward;
        public int Score => _score;
        public int GoldReward => _goldReward;
        public float MoveSpeed => _moveSpeed;
        public float AttackDistance => _attackDistance;
        public float AttackDelay => _attackDelay;
        public float[] AbilityCoolDown => _abilityCoolDown;
        public int[] AbilityDamage => _abilityDamage;
        //public ParticleSystem EnemyHitParticleSystem => _enemyHitParticleSystem;
        public PoolParticle EnemyDieParticleSystem => _enemyDieParticleSystem;
        //public ParticleSystem EnemyAbilityParticleSystem => _enemyAbilityParticleSystem;
        public AudioClip AudioClipDie => _audioClipDie;
        public AudioClip Hit => _hit;
        public Enemy PrefabEnemy => _prefabEnemy;
        public TypeEnemy TypeEnemy => _type;
    }
}