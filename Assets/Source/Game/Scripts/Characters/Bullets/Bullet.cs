using System.Collections;
using UnityEngine;

public class Bullet : PoolObject
{
    private readonly float _lifeTimeBullet = 6f;

    private Coroutine _coroutine;
    protected float _damage;

    public void Initialaze(int damage)
    {
        _damage = damage;

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(LifeTimeCounter());
    }

    private IEnumerator LifeTimeCounter()
    {
        yield return new WaitForSeconds(_lifeTimeBullet);
        ReturnObjectPool();
    }
}