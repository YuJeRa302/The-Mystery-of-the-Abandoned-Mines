using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class TargetAbility : IAttackAbilityStrategy, ILoopCoroutineStrategy
    {
        private readonly float _searchRadius = 20f;

        private ICoroutineRunner _coroutineRunner;
        private Coroutine _damageDealCoroutine;
        private Spell _spellPrefab;
        private Spell _spell;
        private ParticleSystem _particleSystem;
        private Ability _ability;
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
            Vector3 targetPosition;

            if (TryFindEnemy(out Enemy enemy))
                targetPosition = enemy.transform.position;
            else
                targetPosition = _player.transform.position;

            _spell = Object.Instantiate(
                _spellPrefab,
                targetPosition,
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

        private bool TryFindEnemy(out Enemy enemy)
        {
            Collider[] colliderEnemy = Physics.OverlapSphere(
                _player.transform.position,
                _searchRadius);

            foreach (Collider collider in colliderEnemy)
            {
                if (collider.TryGetComponent(out enemy))
                    return true;
            }

            enemy = null;
            return false;
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                {
                    if (_spell.TryFindEnemy(out Enemy enemy))
                        enemy.TakeDamage(_ability.DamageSource);
                }

                yield return new WaitForSeconds(_damageDelay);
            }
        }
    }
}