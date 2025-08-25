using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class ThrowAxeAbilityPresenter : ClassAbilityPresenter, IAbilityPauseStrategy
    {
        private readonly float _delayThrowAxe = 0.3f;

        private ICoroutineRunner _coroutineRunner;
        private Transform _throwPoint;
        private Pool _pool;
        private AxeMissile _axeMissilePrefab;
        private AxeMissile _axeMissile;
        private Coroutine _damageDealCoroutine;
        private Ability _ability;
        private Player _player;

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            ThrowAxeClassAbility throwAxeClass = abilityEntitiesHolder.AttributeData as ThrowAxeClassAbility;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _pool = _player.Pool;
            _axeMissilePrefab = throwAxeClass.AxeMissile;
            _throwPoint = _player.ThrowAbilityPoint;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);
            Spawn();

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

        private void Spawn()
        {
            if (TryFindSummon(_axeMissilePrefab.gameObject, out AxeMissile poolAxe))
            {
                _axeMissile = poolAxe;
                _axeMissile.transform.position = _throwPoint.position;
                _axeMissile.gameObject.SetActive(true);
                _axeMissile.ThrowNow();
            }
            else
            {
                _axeMissile = GameObject.Instantiate(_axeMissilePrefab, _throwPoint.position, Quaternion.identity);

                _pool.InstantiatePoolObject(_axeMissile, _axeMissilePrefab.name);
                _axeMissile.Initialize(_player,
                    _player.DamageSource,
                    _player.PlayerStats.MoveSpeed,
                    _ability.CurrentDuration);
            }

            _axeMissile.GetComponent<Rigidbody>().AddForce(
                _throwPoint.forward * _ability.CurrentDuration,
                ForceMode.Impulse);
        }

        private bool TryFindSummon(GameObject type, out AxeMissile poolObj)
        {
            poolObj = null;

            if (_pool.TryPoolObject(type, out PoolObject axePool))
            {
                poolObj = axePool as AxeMissile;
            }

            return poolObj != null;
        }

        private IEnumerator DealDamage()
        {
            while (_ability.IsAbilityEnded == false)
            {
                if (_axeMissile != null)
                {
                    if (_axeMissile.TryFindEnemies(out List<Enemy> enemies))
                    {
                        foreach (var enemy in enemies)
                        {
                            enemy.TakeDamage(_player.DamageSource);
                        }
                    }
                }

                yield return new WaitForSeconds(_delayThrowAxe);
            }
        }
    }
}