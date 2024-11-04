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
        [SerializeField] private int _timeSpawn;
        [SerializeField] private int _enemyCount;
        [Header("[Ability Stats]")]
        [SerializeField] private float[] _abilityCoolDown;
        [SerializeField] private int[] _abilityDamage;
        [Header("[ParticleSystem]")]
        [SerializeField] private ParticleSystem _enemyHitParticleSystem;
        [SerializeField] private ParticleSystem _enemyDieParticleSystem;
        [SerializeField] private ParticleSystem _enemyAbilityParticleSystem;
        [Header("[Enemy Sound]")]
        [SerializeField] private AudioClip _audioClipDie;
        [SerializeField] private AudioClip _hit;
        [Header("[Enemy]")]
        [SerializeField] private Enemy _prefabEnemy;

        public int Id => _id;
        public int UpgradeExperienceReward => _upgradeExperienceReward;
        public int EnemyCount => _enemyCount;
        public int TimeSpawn => _timeSpawn;
        public int Level => _level;
        public int Damage => _damage;
        public int Health => _health;
        public int ExperienceReward => _experienceReward;
        public int Score => _score;
        public float[] AbilityCoolDown => _abilityCoolDown;
        public int[] AbilityDamage => _abilityDamage;
        public ParticleSystem EnemyHitParticleSystem => _enemyHitParticleSystem;
        public ParticleSystem EnemyDieParticleSystem => _enemyDieParticleSystem;
        public ParticleSystem EnemyAbilityParticleSystem => _enemyAbilityParticleSystem;
        public AudioClip AudioClipDie => _audioClipDie;
        public AudioClip Hit => _hit;
        public Enemy PrefabEnemy => _prefabEnemy;
    }
}