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
    public class ThunderPresenter : IAbilityStrategy, IAbilityPauseStrategy
    {
        private readonly float _delayAttack = 0.3f;

        private ICoroutineRunner _coroutineRunner;
        private LegendaryThunderAbilitySpell _spellPrefab;
        private LegendaryThunderAbilitySpell _spell;
        private Coroutine _damageDealCoroutine;
        private ParticleSystem _particleSystem;
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
            ThrowBlast();

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void EndedAbility(Ability ability)
        {
            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        public void PausedGame(bool state)
        {
            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        public void ResumedGame(bool state)
        {
            if (_damageDealCoroutine != null)
                _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        private void ThrowBlast()
        {
            _spell = Object.Instantiate(
                    _spellPrefab,
                    new Vector3(
                        _player.transform.position.x,
                        _spellPrefab.transform.position.y,
                        _player.transform.position.z),
                    Quaternion.identity);

            (_spell as LegendadatyTunderAbilitySpell).Initialize(_particleSystem, _ability.CurrentDuration);
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                yield return new WaitForSeconds(_delayAttack);

                if (_spell != null)
                {
                    if ((_spell as LegendadatyTunderAbilitySpell).TryFindEnemies(out List<Enemy> enemies))
                    {
                        foreach (var enemy in enemies)
                        {
                            enemy.TakeDamage(_ability.DamageSource);
                        }

                        Object.Destroy(_spell);
                    }
                }
            }
        }
    }
}