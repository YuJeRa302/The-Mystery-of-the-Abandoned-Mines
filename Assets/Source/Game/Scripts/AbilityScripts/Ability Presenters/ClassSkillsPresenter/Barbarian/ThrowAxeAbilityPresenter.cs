using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class ThrowAxeAbilityPresenter : AbilityPresenter
    {
        private Transform _throwPoint;
        private Pool _pool;
        private AxeMissile _axeMissilePrefab;
        private AxeMissile _axeMissile;
        private bool _isAbilityUse;
        private Coroutine _damageDealCoroutine;

        public ThrowAxeAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            GamePauseService gamePauseService,
            GameLoopService gameLoopService,
            ICoroutineRunner coroutineRunner,
            AxeMissile axeMissile) : base(ability, abilityView, player,
                gamePauseService, gameLoopService, coroutineRunner)
        {
            _throwPoint = Player.ThrowAbilityPoint;
            _pool = Player.Pool;
            _axeMissilePrefab = axeMissile;
            AddListener();
        }

        protected override void AddListener()
        {
            base.AddListener();
            (AbilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
        }

        protected override void RemoveListener()
        {
            base.RemoveListener();
            (AbilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
        }

        protected override void OnAbilityEnded(Ability ability)
        {
            _isAbilityUse = false;

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);
        }

        private void OnButtonSkillClick()
        {
            if (_isAbilityUse)
                return;

            _isAbilityUse = true;
            Ability.Use();
            (AbilityView as ClassSkillButtonView).SetInteractableButton(false);
        }

        protected override void OnAbilityUsed(Ability ability)
        {
            _isAbilityUse = true;
            Spawn();

            if (_damageDealCoroutine != null)
                CoroutineRunner.StopCoroutine(_damageDealCoroutine);

            _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        protected override void OnGameResumed(bool state)
        {
            base.OnGameResumed(state);

            if (_damageDealCoroutine != null)
                _damageDealCoroutine = CoroutineRunner.StartCoroutine(DealDamage());
        }

        protected override void OnCooldownValueReset(float value)
        {
            base.OnCooldownValueReset(value);
            (AbilityView as ClassSkillButtonView).SetInteractableButton(true);
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
                _axeMissile.Initialize(Player, Player.DamageSource, Player.MoveSpeed, Ability.CurrentDuration);
            }

            _axeMissile.GetComponent<Rigidbody>().AddForce(_throwPoint.forward * Ability.CurrentDuration,
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
            while (Ability.IsAbilityEnded == false)
            {
                if (_axeMissile != null)
                {
                    if (_axeMissile.TryFindEnemies(out List<Enemy> enemies))
                    {
                        foreach (var enemy in enemies)
                        {
                            enemy.TakeDamage(Player.DamageSource);
                        }
                    }
                }

                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}