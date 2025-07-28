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
    public class SnowfallPresenter : IAbilityStrategy, IAbilityPauseStrategy
    {
        private readonly float _delayThrowing = 0.5f;
        private readonly float _delayAttack = 0.3f;
        private readonly float _blastSpeed = 12f;
        private readonly float _searchRadius = 20f;
        private readonly int _countSpell = 3;

        private ICoroutineRunner _coroutineRunner;
        private LegendarySpell _spellPrefab;
        private LegendarySpell _spell;
        private List<LegendarySpell> _spawnedSpell = new ();
        private Vector3 _direction;
        private Coroutine _blastThrowingCoroutine;
        private Coroutine _damageDealCoroutine;
        private Coroutine _throwSpellCoroutine;
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
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public void UsedAbility(Ability ability)
        {
            if (_throwSpellCoroutine != null)
                _coroutineRunner.StopCoroutine(_throwSpellCoroutine);

            _throwSpellCoroutine = _coroutineRunner.StartCoroutine(ThrowBlast());

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void EndedAbility(Ability ability)
        {
            if (_throwSpellCoroutine != null)
                _coroutineRunner.StopCoroutine(_throwSpellCoroutine);

            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _spawnedSpell.Clear();
        }

        public void PausedGame(bool state)
        {
            if (_blastThrowingCoroutine != null)
                _coroutineRunner.StopCoroutine(_blastThrowingCoroutine);

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            if (_throwSpellCoroutine != null)
                _coroutineRunner.StopCoroutine(_throwSpellCoroutine);
        }

        public void ResumedGame(bool state)
        {
            if (_blastThrowingCoroutine != null)
                _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

            if (_damageDealCoroutine != null)
                _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());

            if (_throwSpellCoroutine != null)
                _throwSpellCoroutine = _coroutineRunner.StartCoroutine(ThrowBlast());
        }

        private bool TryFindEnemy(out Enemy enemy)
        {
            Collider[] colliderEnemy = Physics.OverlapSphere(_player.transform.position, _searchRadius);

            foreach (Collider collider in colliderEnemy)
            {
                if (collider.TryGetComponent(out enemy))
                    return true;
            }

            enemy = null;
            return false;
        }

        private IEnumerator ThrowBlast()
        {
            for (int i = 0; i < _countSpell; i++)
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
                _spawnedSpell.Add(_spell);
                _blastThrowingCoroutine = _coroutineRunner.StartCoroutine(ThrowingBlast());

                yield return new WaitForSeconds(_delayThrowing);
            }
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                yield return new WaitForSeconds(_delayAttack);

                for (int i = 0; i < _spawnedSpell.Count; i++)
                {
                    if (_spawnedSpell[i] != null)
                    {
                        if (_spawnedSpell[i].TryFindEnemy(out Enemy enemy))
                        {
                            enemy.TakeDamage(_ability.DamageSource);
                        }
                    }
                }
            }
        }

        private IEnumerator ThrowingBlast()
        {
            while (_ability.IsAbilityEnded == false)
            {
                for (int i = 0; i < _spawnedSpell.Count; i++)
                {
                    if (_spawnedSpell[i] != null)
                        _spawnedSpell[i].transform.Translate(_direction * _blastSpeed * Time.deltaTime);
                    else
                        yield return null;
                }

                yield return null;
            }
        }
    }
}