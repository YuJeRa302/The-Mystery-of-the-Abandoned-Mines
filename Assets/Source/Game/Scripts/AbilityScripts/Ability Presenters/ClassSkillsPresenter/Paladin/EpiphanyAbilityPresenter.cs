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
    public class EpiphanyAbilityPresenter : ClassAbilityPresenter
    {
        private ICoroutineRunner _coroutineRunner;
        private Coroutine _damageDealCoroutine;
        private ParticleSystem _poolParticle;
        private Spell _spell;
        private Spell _spellPrefab;
        private float _spellRadius = 8f;
        private float _damageDelay;
        private Ability _ability;
        private Player _player;

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            EpiphanyClassAbilityData epiphanyClassData = abilityEntitiesHolder.AttributeData as EpiphanyClassAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _poolParticle = epiphanyClassData.EpiphanyParticle;
            _spellPrefab = epiphanyClassData.Spell;
            _damageDelay = epiphanyClassData.DamageSource.DamageDelay;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);
            CreateSpell();

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public override void EndedAbility(Ability ability)
        {
            base.EndedAbility(ability);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        private void CreateSpell()
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

                yield return new WaitForSeconds(_damageDelay);
            }
        }
    }
}