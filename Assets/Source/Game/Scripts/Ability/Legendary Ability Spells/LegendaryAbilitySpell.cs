using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LegendaryAbilitySpell : MonoBehaviour
    {
        [SerializeField] protected AbilityEnemyFinder[] _abilityEnemyFinders;

        protected List<Enemy> _enemies = new List<Enemy>();
        protected List<ParticleSystem> _abilityEffects = new ();
        protected float _spellLifeTime;

        public virtual void Initialize(ParticleSystem particleSystem, float currentDuration)
        {
            if (_abilityEffects.Count > 0)
            {
                foreach (var abilityEffect in _abilityEffects) 
                {
                    if (abilityEffect.gameObject != null)
                        Destroy(abilityEffect);
                }
            }

            _spellLifeTime = currentDuration;
            CreateEffect(particleSystem);
            DestroyEffectsByLifeTime(_spellLifeTime);
            Destroy(gameObject, _spellLifeTime);
        }

        public bool TryFindEnemy(out Enemy enemyFind)
        {
            foreach (AbilityEnemyFinder abilityEnemyFinder in _abilityEnemyFinders) 
            {
                if (abilityEnemyFinder != null)
                {
                    if (abilityEnemyFinder.TryFindEnemy(out Enemy enemy))
                    {
                        enemyFind = enemy;
                        return true;
                    }
                }
            }

            enemyFind = null;
            return false;
        }

        public virtual bool TryFindEnemys(out List<Enemy> enemies)
        {
            _enemies.Clear();
            enemies = new List<Enemy>();

            foreach (AbilityEnemyFinder abilityEnemyFinder in _abilityEnemyFinders)
            {
                if (abilityEnemyFinder != null)
                {
                    if (abilityEnemyFinder.TryFindEnemys(out List<Enemy> findEnemyse))
                    {
                        _enemies.AddRange(findEnemyse);
                    }
                }
            }

            enemies.AddRange(_enemies);
            return enemies.Count > 0;
        }

        private void CreateEffect(ParticleSystem particleSystem)
        {
            foreach (AbilityEnemyFinder abilityEnemyFinder in _abilityEnemyFinders) 
            {
                ParticleSystem abilityEffect = Instantiate(particleSystem, abilityEnemyFinder.transform);
                abilityEffect.Play();
                _abilityEffects.Add(abilityEffect);
            }
        }

        protected virtual void DestroyEffectsByLifeTime(float spellLifeTime) 
        {
            foreach (var abilityEffect in _abilityEffects)
            {
                Destroy(abilityEffect.gameObject, _spellLifeTime);
            }
        }
    }
}