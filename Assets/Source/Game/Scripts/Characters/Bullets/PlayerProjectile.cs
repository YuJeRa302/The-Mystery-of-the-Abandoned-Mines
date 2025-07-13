using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Rooms;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class PlayerProjectile : PoolObject
    {
        private Enemy _target;
        private Vector3 _direction;
        private Coroutine _coroutine;
        private Rigidbody _rigidbody;
        private DamageSource _damageSource;
        private float _projectileMoveSpeed = 2f;
        private float _bulletLifetime = 6f;

        private void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position,
                _target.transform.position, 5f * Time.fixedDeltaTime);
            _rigidbody.velocity = Vector3.zero;
            _direction = new Vector3(_target.transform.position.x,
                _target.transform.position.y,
                _target.transform.position.z).normalized;
            _rigidbody.AddForce(_direction * _projectileMoveSpeed);
            _direction = Vector3.zero;

            if (_rigidbody.velocity.y < 0f)
                _rigidbody.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

            Vector3 horizontalVelocity = _rigidbody.velocity;
            horizontalVelocity.y = 0;

            if (horizontalVelocity.sqrMagnitude > _projectileMoveSpeed * _projectileMoveSpeed)
                _rigidbody.velocity = horizontalVelocity.normalized *
                    _projectileMoveSpeed + Vector3.up * _rigidbody.velocity.y;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damageSource);
                ReturnObjectPool();
            }

            if (collision.collider.TryGetComponent(out Wall wall))
                ReturnObjectPool();
        }

        public void Initialize(Enemy target, DamageSource damageSource)
        {
            _target = target;
            _damageSource = damageSource;

            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(LifeTimeCounter());
        }
        private IEnumerator LifeTimeCounter()
        {
            yield return new WaitForSeconds(_bulletLifetime);
            ReturnObjectPool();
        }
    }
}