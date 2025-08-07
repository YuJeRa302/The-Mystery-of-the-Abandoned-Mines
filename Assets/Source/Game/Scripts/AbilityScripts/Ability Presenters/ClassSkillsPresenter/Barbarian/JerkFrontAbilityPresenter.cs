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
    public class JerkFrontAbilityPresenter : ClassAbilityPresenter, IAbilityPauseStrategy
    {
        private ICoroutineRunner _coroutineRunner;
        private Coroutine _coroutine;
        private Rigidbody _rigidbodyPlayer;
        private Transform _effectContainer;
        private Pool _pool;
        private PoolParticle _poolParticle;
        private List<PoolObject> _spawnedEffects = new ();
        private Ability _ability;
        private Player _player;

        public override void Construct(AbilityEntitiesHolder abilityEntitiesHolder)
        {
            base.Construct(abilityEntitiesHolder);
            JerkFrontAbilityData jerkFrontAbilityData = abilityEntitiesHolder.AttributeData as JerkFrontAbilityData;
            _ability = abilityEntitiesHolder.Ability;
            _player = abilityEntitiesHolder.Player;
            _pool = _player.Pool;
            _poolParticle = jerkFrontAbilityData.PoolParticle;
            _effectContainer = _player.PlayerAbilityContainer;
            _rigidbodyPlayer = _player.GetComponent<Rigidbody>();
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public override void UsedAbility(Ability ability)
        {
            base.UsedAbility(ability);
            ChangeAbilityEffect(IsAbilityUse);
            Jerk();
        }

        public override void EndedAbility(Ability ability)
        {
            base.EndedAbility(ability);

            if (_coroutine != null)
                _coroutineRunner.StopCoroutine(_coroutine);

            ChangeAbilityEffect(IsAbilityUse);
        }

        public void PausedGame(bool state)
        {
            if (_coroutine != null)
                _coroutineRunner.StopCoroutine(_coroutine);
        }

        public void ResumedGame(bool state)
        {
            if (_coroutine != null)
                _coroutineRunner.StopCoroutine(_coroutine);

            _coroutine = _coroutineRunner.StartCoroutine(JerkForward());
        }

        private void Jerk()
        {
            if (_coroutine != null)
                _coroutineRunner.StopCoroutine(_coroutine);

            _coroutine = _coroutineRunner.StartCoroutine(JerkForward());
        }

        private void ChangeAbilityEffect(bool isAbilityEnded)
        {
            if (isAbilityEnded)
            {
                PoolParticle particle;

                if (_pool.TryPoolObject(_poolParticle.gameObject, out PoolObject poolParticle))
                {
                    particle = poolParticle as PoolParticle;
                    particle.gameObject.SetActive(true);
                }
                else
                {
                    particle = Object.Instantiate(_poolParticle, _effectContainer);
                    _pool.InstantiatePoolObject(particle, _poolParticle.name);
                    _spawnedEffects.Add(particle);
                    (particle as DamageParticle).Initialize(_ability.DamageSource);
                }
            }
            else
            {
                foreach (var particle in _spawnedEffects)
                {
                    if (particle != null) 
                    {
                        if (particle.isActiveAndEnabled)
                            particle.ReturnObjectPool();
                    }
                }
            }
        }

        private IEnumerator JerkForward()
        {
            float currentTime = 0;

            while (currentTime <= _ability.CurrentDuration)
            {
                _rigidbodyPlayer.AddForce(_player.transform.forward * _ability.CurrentAbilityValue, ForceMode.Impulse);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}