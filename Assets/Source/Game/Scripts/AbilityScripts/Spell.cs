using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class Spell : MonoBehaviour
    {
        [SerializeField] private Transform _effectContainer;
        [SerializeField] private float _findEnemyRange = 4f;

        private ParticleSystem _abilityEffect;
        private float _spellLifeTime;
        private Collider[] _foundEnemyColliders = new Collider[50];

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _findEnemyRange);
        }

        public void Initialize(ParticleSystem particleSystem, float currentDuration, float radius)
        {
            if (_abilityEffect != null)
            {
                if (_abilityEffect.gameObject != null)
                    Destroy(_abilityEffect);
            }

            _findEnemyRange = radius;
            _spellLifeTime = currentDuration;
            CreateEffect(particleSystem);
            Destroy(_abilityEffect.gameObject, _spellLifeTime);
            Destroy(gameObject, _spellLifeTime);
        }

        public bool TryFindEnemy(out Enemy enemy)
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _findEnemyRange,
                _foundEnemyColliders
            );

            for (int i = 0; i < count; i++)
            {
                if (_foundEnemyColliders[i] != null &&
                    _foundEnemyColliders[i].TryGetComponent(out Enemy findedEnemy))
                {
                    enemy = findedEnemy;
                    return true;
                }

            }

            enemy = null;
            return false;
        }

        public virtual bool TryFindEnemies(out List<Enemy> enemies)
        {
            enemies = new List<Enemy>();
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                _findEnemyRange,
                _foundEnemyColliders
            );

            for (int i = 0; i < count; i++)
            {
                if (_foundEnemyColliders[i] != null &&
                    _foundEnemyColliders[i].TryGetComponent(out Enemy enemy))
                {
                    enemies.Add(enemy);
                }
            }

            return enemies.Count > 0;
        }

        private void CreateEffect(ParticleSystem particleSystem)
        {
            _abilityEffect = Instantiate(particleSystem, transform);
            _abilityEffect.Play();
        }
    }
}