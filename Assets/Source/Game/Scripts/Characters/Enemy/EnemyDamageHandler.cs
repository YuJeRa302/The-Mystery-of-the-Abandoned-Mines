using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyDamageHandler
    {
        private readonly System.Random _rnd = new ();
        private Coroutine _stunDamage;
        private Coroutine _burnDamage;
        private Coroutine _slowDamage;
        private Coroutine _repulsiveDamage;
        private ICoroutineRunner _coroutineRunner;
        private Pool _pool;
        private List<PoolObject> _spawnedEffects = new ();
        private Transform _damageEffectContainer;
        private EnemyHealth _health;
        private Rigidbody _rigidbody;

        public event Action Stuned;
        public event Action StunEnded;
        public event Action MoveSpeedReseted;
        public event Action<float> MoveSpeedReduced;

        public EnemyDamageHandler(Pool pool, Transform effectContainer, Enemy enemy)
        {
            _pool = pool;
            _damageEffectContainer = effectContainer;
            _health = enemy.EnemyHealth;
            _rigidbody = enemy.Rigidbody;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();
        }

        public void CreateDamageEffect(DamageSource damageSource)
        {
            ExtractDamageParameters(damageSource, out float chance, out float duration,
                out float repulsive, out float gradual,
                out float slowDown);

            switch (damageSource.TypeDamage)
            {
                case TypeDamage.PhysicalDamage:
                    break;
                case TypeDamage.StunDamage:
                    TryApplyEffect(chance, () =>
                        _stunDamage = _coroutineRunner.StartCoroutine(Stun(duration, damageSource.PoolParticle)),
                        _stunDamage);
                    break;
                case TypeDamage.RepulsiveDamage:
                    _repulsiveDamage = RestartCoroutine(_repulsiveDamage,
                        () => Repulsive(repulsive));
                    break;
                case TypeDamage.BurningDamage:
                    _burnDamage = RestartCoroutine(_burnDamage,
                        () => Burn(gradual, duration, damageSource.PoolParticle));
                    break;
                case TypeDamage.SlowedDamage:
                    _slowDamage = RestartCoroutine(_slowDamage,
                        () => Slowed(duration, slowDown, damageSource.PoolParticle));
                    break;
            }
        }

        public void Disable()
        {
            foreach (var spawnedParticle in _spawnedEffects)
            {
                spawnedParticle.ReturnObjectPool();
            }

            CoroutineStop(_slowDamage);
            CoroutineStop(_stunDamage);
            CoroutineStop(_repulsiveDamage);
            CoroutineStop(_burnDamage);
        }

        private void ExtractDamageParameters(
                DamageSource damageSource,
                out float chance, out float duration,
                out float repulsive, out float gradual,
                out float slowDown)
        {
            chance = 0;
            duration = 0;
            repulsive = 0;
            gradual = 0;
            slowDown = 0;

            foreach (var parameter in damageSource.DamageParameters)
            {
                switch (parameter.TypeDamageParameter)
                {
                    case TypeDamageParameter.Chance:
                        chance = parameter.Value;
                        break;
                    case TypeDamageParameter.Duration:
                        duration = parameter.Value;
                        break;
                    case TypeDamageParameter.Repulsive:
                        repulsive = parameter.Value;
                        break;
                    case TypeDamageParameter.Gradual:
                        gradual = parameter.Value;
                        break;
                    case TypeDamageParameter.Slowdown:
                        slowDown = parameter.Value;
                        break;
                }
            }
        }

        private void TryApplyEffect(float chance, Action effectAction, Coroutine runningCoroutine)
        {
            if (_rnd.Next(0, 100) <= chance)
            {
                CoroutineStop(runningCoroutine);
                effectAction();
            }
        }

        private void CoroutineStop(Coroutine coroutine)
        {
            if (coroutine != null)
                _coroutineRunner.StopCoroutine(coroutine);
        }

        private Coroutine RestartCoroutine(Coroutine runningCoroutine, Func<IEnumerator> coroutineMethod)
        {
            CoroutineStop(runningCoroutine);

            return _coroutineRunner.StartCoroutine(coroutineMethod());
        }

        private void CreateDamageParticle(PoolParticle poolParticle)
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(poolParticle.gameObject, out PoolObject pollParticle))
            {
                particle = pollParticle as PoolParticle;
                particle.transform.position = _damageEffectContainer.position;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = UnityEngine.Object.Instantiate(poolParticle, _damageEffectContainer);
                _pool.InstantiatePoolObject(particle, poolParticle.name);
                _spawnedEffects.Add(particle);
            }
        }

        private void DisableParticle(PoolParticle particle)
        {
            foreach (var spawnedParticle in _spawnedEffects)
            {
                if (particle.name == spawnedParticle.NameObject)
                    if (spawnedParticle.isActiveAndEnabled)
                        spawnedParticle.ReturnObjectPool();
            }
        }

        private IEnumerator Burn(float damage, float time, PoolParticle particle)
        {
            float currentTime = 0;
            float pastSeconds = 0;
            float delayDamage = 1f;

            CreateDamageParticle(particle);

            while (currentTime <= time)
            {
                pastSeconds += Time.deltaTime;

                if (pastSeconds >= delayDamage)
                {
                    _health.ApplyDamage(damage);
                    pastSeconds = 0;
                    currentTime++;
                }

                yield return null;
            }

            DisableParticle(particle);
        }

        private IEnumerator Repulsive(float value)
        {
            float currentTime = 0;
            _rigidbody.isKinematic = false;

            while (currentTime <= 0.17f)
            {
                _rigidbody.AddForce(_rigidbody.transform.forward * -value, ForceMode.Impulse);
                currentTime += Time.deltaTime;
                yield return null;
            }

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        private IEnumerator Stun(float duration, PoolParticle particle)
        {
            CreateDamageParticle(particle);
            Stuned?.Invoke();
            yield return new WaitForSeconds(duration);
            StunEnded?.Invoke();
            DisableParticle(particle);
        }

        private IEnumerator Slowed(float duration, float valueSlowed, PoolParticle particle)
        {
            float currentTime = 0;
            CreateDamageParticle(particle);
            MoveSpeedReduced?.Invoke(valueSlowed);

            while (currentTime <= duration)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }

            MoveSpeedReseted?.Invoke();
            DisableParticle(particle);
        }
    }
}