using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using UniRx;

public class SlowedDamageHandler : IDamageEffectHandler
{
    private ICoroutineRunner _coroutineRunner;
    private Coroutine _slowDamage;

    public SlowedDamageHandler(ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
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
        MessageBroker.Default.Publish(new M_CreateDamageParticle(particle));
        MessageBroker.Default.Publish(new M_MoveSpeedReduced(valueSlowed));

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        MessageBroker.Default.Publish(new M_MoveSpeedReseted());
        MessageBroker.Default.Publish(new M_DisableParticle(particle));
    }
}