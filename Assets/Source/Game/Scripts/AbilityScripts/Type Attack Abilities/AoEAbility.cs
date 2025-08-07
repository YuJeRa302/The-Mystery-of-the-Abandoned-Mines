using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AoEAbility : IAttackAbilityStrategy, ILoopCoroutineStrategy
    {
        private ICoroutineRunner _coroutineRunner;
        private Spell _spellPrefab;
        private Spell _spell;
        private ParticleSystem _particleSystem;
        private Ability _ability;
        private Coroutine _damageDealCoroutine;
        private Player _player;
        private float _damageDelay;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            AttackAbilityData attackAbilityData = abilityEntitiesHolder.AttributeData as AttackAbilityData;
            _damageDelay = attackAbilityData.DamageSource.DamageDelay;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _particleSystem = abilityEntitiesHolder.ParticleSystem;
            _spellPrefab = attackAbilityData.Spell;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public void Create()
        {
            _spell = Object.Instantiate(
                _spellPrefab,
                new Vector3(
                    _player.transform.position.x,
                    _player.transform.position.y,
                    _player.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(_particleSystem, _ability.CurrentDuration, _ability.SpellRadius);
            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void StartCoroutine()
        {
            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void StopCoroutine()
        {
            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    if (_spell.TryFindEnemies(out List<Enemy> enemies))
                    {
                        foreach (var enemy in enemies)
                        {
                            enemy.TakeDamage(_ability.DamageSource);
                        }
                    }
                }

                yield return new WaitForSeconds(_damageDelay);
            }
        }
    }
}