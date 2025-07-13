using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Create Enemy", order = 51)]
    public class EnemyData : ScriptableObject
    {
        [Header("[Enemy Stats]")]
        [SerializeField] private int _level;
        [SerializeField] private List<EnemyStats> _enemyStats;
        [Header("[ParticleSystem]")]
        [SerializeField] private PoolParticle _enemyDieParticleSystem;
        [Header("[Enemy Sound]")]
        [SerializeField] private AudioClip _audioClipDie;
        [SerializeField] private AudioClip _hit;
        [Header("[Enemy]")]
        [SerializeField] private Enemy _prefabEnemy;
        [SerializeField] private TypeEnemy _type;
        [Header("[Knowleadge]")]
        [SerializeField] private Sprite _enemyIcon;
        [SerializeField] private string _descripton;
        [SerializeField] private string _name;

        public PoolParticle EnemyDieParticleSystem => _enemyDieParticleSystem;
        public AudioClip AudioClipDie => _audioClipDie;
        public AudioClip Hit => _hit;
        public Enemy PrefabEnemy => _prefabEnemy;
        public Sprite Icon => _enemyIcon;
        public string Descroption => _descripton;
        public string Name => _name;
        public List<EnemyStats> EnemyStats => _enemyStats;
    }
}