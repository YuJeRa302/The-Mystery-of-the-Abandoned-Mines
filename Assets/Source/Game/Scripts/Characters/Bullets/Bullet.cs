using Assets.Source.Game.Scripts.PoolSystem;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class Bullet : PoolObject
    {
        private readonly float _lifeTimeBullet = 6f;

        private float _damage;
        private Coroutine _coroutine;

        protected float Damage => _damage;

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
}