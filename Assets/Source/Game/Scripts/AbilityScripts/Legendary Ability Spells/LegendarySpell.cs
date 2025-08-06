using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class LegendarySpell : MonoBehaviour
    {
        [SerializeField] private AbilityEnemyFinder[] _abilityEnemyFinders;

        private List<Enemy> _enemies = new ();
        private List<ParticleSystem> _abilityEffects = new ();
        private float _spellLifeTime;

        protected AbilityEnemyFinder[] AbilityEnemyFinders => _abilityEnemyFinders;
        protected List<Enemy> Enemies => _enemies;
        protected List<ParticleSystem> AbilityEffects => _abilityEffects;
        protected float SpellLifeTime => _spellLifeTime;

        public virtual void Initialize(ParticleSystem particleSystem, float currentDuration)
        {
            if (AbilityEffects.Count > 0)
            {
                foreach (var abilityEffect in AbilityEffects)
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

        public virtual bool TryFindEnemies(out List<Enemy> enemies)
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

        protected void SetSpellLifeTime(float lifeTime) => _spellLifeTime = lifeTime;

        private void CreateEffect(ParticleSystem particleSystem)
        {
            foreach (AbilityEnemyFinder abilityEnemyFinder in _abilityEnemyFinders)
            {
                ParticleSystem abilityEffect = Instantiate(particleSystem, abilityEnemyFinder.transform);
                abilityEffect.Play();
                AbilityEffects.Add(abilityEffect);
            }
        }

        protected virtual void DestroyEffectsByLifeTime(float spellLifeTime)
        {
            foreach (var abilityEffect in AbilityEffects)
            {
                Destroy(abilityEffect.gameObject, _spellLifeTime);
            }
        }
    }
}