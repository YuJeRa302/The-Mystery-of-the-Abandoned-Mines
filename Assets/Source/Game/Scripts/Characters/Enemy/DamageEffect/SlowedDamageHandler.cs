using Assets.Source.Game.Scripts.Enums;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using UniRx;

namespace Assets.Source.Game.Scripts.Characters
{
    public class SlowedDamageHandler : IDamageEffectHandler
    {
        private ICoroutineRunner _coroutineRunner;
        private Coroutine _slowDamage;
        private EnemyDamageHandler _enemyDamageHandler;
        private Enemy _enemy;

        public SlowedDamageHandler(ICoroutineRunner coroutineRunner, EnemyDamageHandler enemyDamageHandler, Enemy enemy)
        {
            _coroutineRunner = coroutineRunner;
            _enemyDamageHandler = enemyDamageHandler;
            _enemy = enemy;
        }

        public void ApplayDamageEffect(DamageSource damageSource, Dictionary<TypeDamageParameter, float> extractDamage)
        {
            extractDamage.TryGetValue(TypeDamageParameter.Slowdown, out float slowDown);
            extractDamage.TryGetValue(TypeDamageParameter.Duration, out float duration);

            _slowDamage = RestartCoroutine(_slowDamage,
                () => Slowed(duration, slowDown, damageSource.PoolParticle));
        }

        private Coroutine RestartCoroutine(Coroutine runningCoroutine, Func<IEnumerator> coroutineMethod)
        {
            CoroutineStop(runningCoroutine);

            return _coroutineRunner.StartCoroutine(coroutineMethod());
        }

        private void CoroutineStop(Coroutine coroutine)
        {
            if (coroutine != null)
                _coroutineRunner.StopCoroutine(coroutine);
        }

        private IEnumerator Slowed(float duration, float valueSlowed, PoolParticle particle)
        {
            float currentTime = 0;
            MessageBroker.Default.Publish(new M_CreateDamageParticle(particle, _enemyDamageHandler));
            MessageBroker.Default.Publish(new M_MoveSpeedReduced(valueSlowed, _enemy));

            while (currentTime <= duration)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }

            MessageBroker.Default.Publish(new M_MoveSpeedReseted(_enemy));
            MessageBroker.Default.Publish(new M_DisableParticle(particle, _enemyDamageHandler));
        }
    }
}