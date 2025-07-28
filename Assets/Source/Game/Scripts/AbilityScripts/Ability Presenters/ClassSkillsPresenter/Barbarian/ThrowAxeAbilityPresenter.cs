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
    public class ThrowAxeAbilityPresenter : IAbilityStrategy, IClassAbilityStrategy, IAbilityPauseStrategy
    {
        private readonly float _delayThrowAxe = 0.3f;

        private ICoroutineRunner _coroutineRunner;
        private Transform _throwPoint;
        private Pool _pool;
        private AxeMissile _axeMissilePrefab;
        private AxeMissile _axeMissile;
        private bool _isAbilityUse = false;
        private Coroutine _damageDealCoroutine;
        private Ability _ability;
        private AbilityView _abilityView;
        private Player _player;

        public void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            ThrowAxeClassAbility throwAxeClass = abilityEntitiesHolder.AttributeData as ThrowAxeClassAbility;
            _ability = abilityEntitiesHolder.Ability;
            _abilityView = abilityEntitiesHolder.AbilityView;
            _player = abilityEntitiesHolder.Player;
            _pool = _player.Pool;
            _axeMissilePrefab = throwAxeClass.AxeMissile;
            _throwPoint = _player.ThrowAbilityPoint;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public void UsedAbility(Ability ability)
        {
            if (_isAbilityUse == false)
                return;

            Spawn();

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = _coroutineRunner.StartCoroutine(DealDamage());
        }

        public void EndedAbility(Ability ability)
        {
            _isAbilityUse = false;

            if (_damageDealCoroutine != null)
                _coroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        public void AddListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
        }

        public void RemoveListener()
        {
            (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
        }

        public void SetInteractableButton()
        {
            (_abilityView as ClassSkillButtonView).SetInteractableButton(true);
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

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            _ability.Use();
            (_abilityView as ClassSkillButtonView).SetInteractableButton(false);
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
                _axeMissile.Initialize(_player, _player.DamageSource, _player.MoveSpeed, _ability.CurrentDuration);
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