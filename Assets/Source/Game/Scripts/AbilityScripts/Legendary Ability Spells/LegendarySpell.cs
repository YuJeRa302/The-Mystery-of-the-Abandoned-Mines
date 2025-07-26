using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class LegendarySpell : MonoBehaviour
    {
        [SerializeField] protected AbilityEnemyFinder[] AbilityEnemyFinders;

        protected List<Enemy> Enemies = new();
        protected List<ParticleSystem> AbilityEffects = new();
        protected float SpellLifeTime;

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

            SpellLifeTime = currentDuration;
            CreateEffect(particleSystem);
            DestroyEffectsByLifeTime(SpellLifeTime);
            Destroy(gameObject, SpellLifeTime);
        }

        public bool TryFindEnemy(out Enemy enemyFind)
        {
            foreach (AbilityEnemyFinder abilityEnemyFinder in AbilityEnemyFinders)
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
            Enemies.Clear();
            enemies = new List<Enemy>();

            foreach (AbilityEnemyFinder abilityEnemyFinder in AbilityEnemyFinders)
            {
                if (abilityEnemyFinder != null)
                {
                    if (abilityEnemyFinder.TryFindEnemys(out List<Enemy> findEnemyse))
                    {
                        Enemies.AddRange(findEnemyse);
                    }
                }
            }

            enemies.AddRange(Enemies);
            return enemies.Count > 0;
        }

        private void CreateEffect(ParticleSystem particleSystem)
        {
            foreach (AbilityEnemyFinder abilityEnemyFinder in AbilityEnemyFinders)
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
                Destroy(abilityEffect.gameObject, SpellLifeTime);
            }
        }
    }
}