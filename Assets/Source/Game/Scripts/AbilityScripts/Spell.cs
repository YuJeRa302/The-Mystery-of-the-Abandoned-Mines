using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class Spell : MonoBehaviour
    {
        protected List<Enemy> Enemies = new();

        [SerializeField] private Transform _effectContainer;
        [SerializeField] private float _findEnemyRange = 4f;

        private ParticleSystem _abilityEffect;
        private float _spellLifeTime;

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
            Collider[] colliderEnemies = Physics.OverlapSphere(transform.position, _findEnemyRange);

            foreach (Collider collider in colliderEnemies)
            {
                if (collider.TryGetComponent(out enemy))
                    return true;
            }

            enemy = null;
            return false;
        }

        public virtual bool TryFindEnemies(out List<Enemy> enemies)
        {
            Enemies.Clear();
            enemies = new List<Enemy>();
            Collider[] colliderEnemies = Physics.OverlapSphere(transform.position, _findEnemyRange);

            foreach (Collider collider in colliderEnemies)
            {
                if (collider.TryGetComponent(out Enemy enemy))
                {
                    Enemies.Add(enemy);
                }
            }

            enemies.AddRange(Enemies);
            return enemies.Count > 0;
        }

        private void CreateEffect(ParticleSystem particleSystem)
        {
            _abilityEffect = Instantiate(particleSystem, transform);
            _abilityEffect.Play();
        }
    }
}