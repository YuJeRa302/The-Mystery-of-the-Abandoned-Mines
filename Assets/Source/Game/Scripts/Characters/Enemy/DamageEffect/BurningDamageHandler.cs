using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Upgrades;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BurningDamageHandler : IDamageEffectHandler
{
    private ICoroutineRunner _coroutineRunner;
    private Coroutine _burnDamage;

    public BurningDamageHandler(ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void ApplayDamageEffect(DamageSource damageSource, Dictionary<TypeDamageParameter, float> extractDamage)
    {
        extractDamage.TryGetValue(TypeDamageParameter.Gradual, out float gradual);
        extractDamage.TryGetValue(TypeDamageParameter.Duration, out float duration);

        _burnDamage = RestartCoroutine(_burnDamage,
            () => Burn(gradual, duration, damageSource.PoolParticle));
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

    private IEnumerator Burn(float damage, float time, PoolParticle particle)
    {
        float currentTime = 0;
        float pastSeconds = 0;
        float delayDamage = 1f;

        MessageBroker.Default.Publish(new M_CreateDamageParticle(particle));

        while (currentTime <= time)
        {
            pastSeconds += Time.deltaTime;

            if (pastSeconds >= delayDamage)
            {
                MessageBroker.Default.Publish(new M_ApplyBurnDamage(damage));
                pastSeconds = 0;
                currentTime++;
            }

            yield return null;
        }

        MessageBroker.Default.Publish(new M_DisableParticle(particle));
    }
}