using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class LegendaryAbilitySpell : MonoBehaviour
    {
        [SerializeField] private AbilityEnemyFinder[] _abilityEnemyFinders;

        private List<ParticleSystem> _abilityEffects = new ();
        private float _spellLifeTime;

        public void Initialize(ParticleSystem particleSystem, float currentDuration)
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
                if (abilityEnemyFinder.TryFindEnemy(out Enemy enemy)) 
                {
                    enemyFind = enemy;
                    return true;
                }
            }

            enemyFind = null;
            return false;
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

        private void DestroyEffectsByLifeTime(float spellLifeTime) 
        {
            foreach (var abilityEffect in _abilityEffects)
            {
                Destroy(abilityEffect.gameObject, _spellLifeTime);
            }
        }
    }
}