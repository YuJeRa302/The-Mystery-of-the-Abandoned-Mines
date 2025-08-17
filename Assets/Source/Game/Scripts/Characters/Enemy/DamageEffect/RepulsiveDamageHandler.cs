using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsiveDamageHandler : IDamageEffectHandler
{
    private readonly float _controlValueTime = 0.17f;

    private ICoroutineRunner _coroutineRunner;
    private Coroutine _repulsiveDamage;
    private Rigidbody _rigidbody;

    public RepulsiveDamageHandler(ICoroutineRunner coroutineRunner, Rigidbody rigidbody)
    {
        _coroutineRunner = coroutineRunner;
        _rigidbody = rigidbody;
    }

    public void ApplayDamageEffect(DamageSource damageSource, Dictionary<TypeDamageParameter, float> extractDamage)
    {
        extractDamage.TryGetValue(TypeDamageParameter.Repulsive, out float repulsive);

        _repulsiveDamage = RestartCoroutine(_repulsiveDamage,
            () => Repulsive(repulsive));
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

    private IEnumerator Repulsive(float value)
    {
        float currentTime = 0;
        _rigidbody.isKinematic = false;

        while (currentTime <= _controlValueTime)
        {
            _rigidbody.AddForce(_rigidbody.transform.forward * -value, ForceMode.Impulse);
            currentTime += Time.deltaTime;
            yield return null;
        }

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
    }
}