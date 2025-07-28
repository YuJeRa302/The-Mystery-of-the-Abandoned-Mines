using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class FirestormPresenter : IAbilityStrategy, IAbilityPauseStrategy
    {
        private readonly float _delayAttack = 0.3f;
        private readonly float _blastSpeed = 12f;
        private readonly float _searchRadius = 20f;

        private ICoroutineRunner _coroutineRunner;
        private LegendaryThunderAbilitySpell _spellPrefab;
        private LegendaryThunderAbilitySpell _spell;
        private Vector3 _direction;
        private Coroutine _blastThrowingCoroutine;
        private Coroutine _damageDealCoroutine;
        private Transform _throwPoint;
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
            _throwPoint = _player.ThrowAbilityPoint;
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
            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        public void PausedGame(bool state)
        {
            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        public void ResumedGame(bool state)
        {
            if (_blastThrowingCoroutine != null)
                _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

            if (_damageDealCoroutine != null)
                _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        private void ThrowBlast()
        {
            _spell = Object.Instantiate(
                _spellPrefab,
                new Vector3(
                    _throwPoint.transform.position.x, 
                    _throwPoint.transform.position.y,
                    _throwPoint.transform.position.z),
                Quaternion.identity);

            if (TryFindEnemy(out Enemy enemy))
            {
                Transform currentTarget = enemy.transform;
                _direction = (currentTarget.position - _player.transform.position).normalized;
            }
            else
            {
                _direction = _throwPoint.forward;
            }

            _spell.Initialize(_particleSystem, _ability.CurrentDuration);
            _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());
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
                yield return new WaitForSeconds(_delayAttack);

                if (_spell != null)
                    if (_spell.TryFindEnemy(out Enemy enemy))
                        enemy.TakeDamage(_ability.DamageSource);
            }
        }

        private IEnumerator ThrowingBlast()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_spell != null)
                    _spell.transform.Translate(_direction * _blastSpeed * Time.deltaTime);
                else
                    yield return null;

                yield return null;
            }
        }
    }
}