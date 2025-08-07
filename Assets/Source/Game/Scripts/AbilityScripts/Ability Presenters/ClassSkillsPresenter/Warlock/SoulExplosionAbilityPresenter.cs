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
    public class SoulExplosionAbilityPresenter : ClassAbilityPresenter, IAbilityPauseStrategy
    {
        private ICoroutineRunner _coroutineRunner;
        private ParticleSystem _poolParticle;
        private Spell _spell;
        private Spell _spellPrefab;
        private Coroutine _damageDealingCoroutine;
        private float _delayDamage;
        private float _spellRadius = 5f;
        private Ability _ability;
        private Player _player;

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            SoulExplosionAbilityData soulExplosionAbilityData = abilityEntitiesHolder.AttributeData as SoulExplosionAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _poolParticle = soulExplosionAbilityData.DamageParticle;
            _spellPrefab = soulExplosionAbilityData.Spell;
            _delayDamage = soulExplosionAbilityData.DamageSource.DamageDelay;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);
            CreateParticle();

            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);

            _damageDealingCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public override void EndedAbility(Ability ability)
        {
            base.EndedAbility(ability);

            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);
        }

        public void PausedGame(bool state)
        {
            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);
        }

        public void ResumedGame(bool state)
        {
            if (_damageDealingCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealingCoroutine);

            _damageDealingCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        private void CreateParticle()
        {
            _spell = Object.Instantiate(
                    _spellPrefab,
                    new Vector3(
                    _player.transform.position.x, 
                    _player.transform.position.y,
                    _player.transform.position.z),
                    Quaternion.identity);

            _spell.Initialize(_poolParticle, _ability.CurrentDuration, _spellRadius);
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

                yield return new WaitForSeconds(_delayDamage);
            }
        }
    }
}