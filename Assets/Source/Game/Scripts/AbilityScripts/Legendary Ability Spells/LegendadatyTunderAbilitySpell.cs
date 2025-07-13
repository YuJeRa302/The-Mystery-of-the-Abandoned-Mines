using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class LegendadatyTunderAbilitySpell : LegendaryThunderAbilitySpell
    {
        protected ParticleSystem Effect;

        public override void Initialize(ParticleSystem particleSystem, float currentDuration)
        {
            if (AbilityEffects.Count > 0)
            {
                foreach (var abilityEffect in AbilityEffects)
                {
                    if (abilityEffect.gameObject != null)
                        Destroy(abilityEffect);
                }
            }

            Effect = particleSystem;
            SpellLifeTime = currentDuration;
            Destroy(gameObject, SpellLifeTime);
        }

        public override bool TryFindEnemies(out List<Enemy> enemies)
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

            if (enemies.Count > 0)
            {
                CreateEffect();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateEffect()
        {
            foreach (Enemy enemy in Enemies)
            {
                ParticleSystem abilityEffect = Instantiate(Effect, enemy.transform.position, Quaternion.identity);
                abilityEffect.Play();
                AbilityEffects.Add(abilityEffect);
            }

            DestroyEffectsByLifeTime(SpellLifeTime);
        }
    }
}