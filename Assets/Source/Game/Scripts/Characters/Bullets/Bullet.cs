using Assets.Source.Game.Scripts.PoolSystem;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class Bullet : PoolObject
    {
        private readonly float _lifeTimeBullet = 6f;

        protected float Damage;

        private Coroutine _coroutine;

        public void Initialaze(int damage)
        {
            Damage = damage;

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
}