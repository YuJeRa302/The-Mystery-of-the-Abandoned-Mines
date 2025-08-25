using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class StunDamageHandler : IDamageEffectHandler
    {
        private readonly System.Random _rnd = new();

        private ICoroutineRunner _coroutineRunner;
        private Coroutine _stunCoroutine;
        private EnemyDamageHandler _enemyDamageHandler;
        private EnemyStateMachineExample _enemyStateMachine;

        public StunDamageHandler(ICoroutineRunner coroutineRunner, EnemyDamageHandler enemyDamageHandler, Enemy enemy)
        {
            _coroutineRunner = coroutineRunner;
            _enemyDamageHandler = enemyDamageHandler;
            _enemyStateMachine = enemy.EnemyStateMachineExample;
        }

        public void ApplayDamageEffect(DamageSource damageSource,
            Dictionary<TypeDamageParameter, float> extractDamage)
        {
            extractDamage.TryGetValue(TypeDamageParameter.Chance, out float chance);
            extractDamage.TryGetValue(TypeDamageParameter.Chance, out float duration);

            TryApplyEffect(
                chance,
                () => _stunCoroutine = _coroutineRunner.StartCoroutine(Stun(duration, damageSource.PoolParticle)),
                _stunCoroutine
            );
        }

        private void CoroutineStop(Coroutine coroutine)
        {
            if (coroutine != null)
                _coroutineRunner.StopCoroutine(coroutine);
        }

        private void TryApplyEffect(float chance, Action effectAction, Coroutine runningCoroutine)
        {
            if (_rnd.Next(0, 100) <= chance)
            {
                CoroutineStop(runningCoroutine);
                effectAction();
            }
        }

        private IEnumerator Stun(float duration, PoolParticle particle)
        {
            MessageBroker.Default.Publish(new M_CreateDamageParticle(particle, _enemyDamageHandler));
            MessageBroker.Default.Publish(new M_Stuned(true, _enemyStateMachine));
            yield return new WaitForSeconds(duration);
            MessageBroker.Default.Publish(new M_Stuned(false, _enemyStateMachine));
            MessageBroker.Default.Publish(new M_DisableParticle(particle, _enemyDamageHandler));
        }
    }
}