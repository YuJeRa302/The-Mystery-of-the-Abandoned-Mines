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
    public class DragonTracePresenter : IAbilityStrategy, IAbilityPauseStrategy
    {
        private readonly float _delayAttack = 0.3f;
        private readonly float _delaySpawnSpell = 0.5f;

        private ICoroutineRunner _coroutineRunner;
        private LegendarySpell _spellPrefab;
        private LegendarySpell _spell;
        private Coroutine _damageDealCoroutine;
        private Coroutine _spawnedSpellCoroutine;
        private ParticleSystem _particleSystem;
        private List<LegendarySpell> _spells = new ();
        private Ability _ability;
        private Player _player;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            LegendaryAbilityData legendaryAbilityData = abilityEntitiesHolder.AttributeData as LegendaryAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _particleSystem = abilityEntitiesHolder.ParticleSystem;
            _spellPrefab = legendaryAbilityData.LegendarySpell;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public void UsedAbility(Ability ability)
        {
            if (_spawnedSpellCoroutine != null)
                _coroutineRunner.StopCoroutine(_spawnedSpellCoroutine);

            _spawnedSpellCoroutine = _coroutineRunner.StartCoroutine(SpawnSpell());

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void EndedAbility(Ability ability)
        {
            if (_spawnedSpellCoroutine != null)
                _coroutineRunner.StopCoroutine(_spawnedSpellCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        public void PausedGame(bool state)
        {
            if (_spawnedSpellCoroutine != null)
                _coroutineRunner.StopCoroutine(_spawnedSpellCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        public void ResumedGame(bool state)
        {
            if (_spawnedSpellCoroutine != null)
                _spawnedSpellCoroutine = _coroutineRunner.StartCoroutine(SpawnSpell());

            if (_damageDealCoroutine != null)
                _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        private IEnumerator SpawnSpell()
        {
            float lastTime = 0;

            while (lastTime < _ability.CurrentDuration)
            {
                _spell = Object.Instantiate(
                    _spellPrefab,
                    new Vector3(
                        _player.transform.position.x, 
                        _player.transform.position.y,
                        _player.transform.position.z),
                    Quaternion.identity);

                _spell.Initialize(_particleSystem, _ability.CurrentDuration);
                _spells.Add(_spell);

                yield return new WaitForSeconds(_delaySpawnSpell);
                lastTime++;
            }
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                yield return new WaitForSeconds(_delayAttack);

                if (_spells.Count > 0)
                {
                    foreach (var spell in _spells)
                    {
                        if (spell != null)
                        {
                            if (spell.TryFindEnemies(out List<Enemy> enemies))
                            {
                                foreach (var enemy in enemies)
                                {
                                    enemy.TakeDamage(_ability.DamageSource);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}