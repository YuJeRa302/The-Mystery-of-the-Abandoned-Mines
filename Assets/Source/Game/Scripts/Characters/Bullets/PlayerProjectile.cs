using Assets.Source.Game.Scripts;
using System.Collections;
using UnityEngine;

public class PlayerProjectile : PoolObject
{
    private Enemy _target;
    private float _damage;
    private Vector3 _direction;
    private Coroutine _coroutine;
    private Rigidbody _rigidbody;
    private DamageParametr _damageParametr;
    private float _moveSpeedBoost;
    private float _projectaleMoveSpeed = 2f;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _target.transform.position, 5f * _moveSpeedBoost * Time.fixedDeltaTime);
        _rigidbody.velocity = Vector3.zero;
        _direction = new Vector3(_target.transform.position.x, _target.transform.position.y, _target.transform.position.z).normalized;
        _rigidbody.AddForce(_direction * _projectaleMoveSpeed * _moveSpeedBoost);
        _direction = Vector3.zero;

        if (_rigidbody.velocity.y < 0f)
            _rigidbody.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = _rigidbody.velocity;
        horizontalVelocity.y = 0;

        if (horizontalVelocity.sqrMagnitude > _projectaleMoveSpeed * _projectaleMoveSpeed)
            _rigidbody.velocity = horizontalVelocity.normalized * _projectaleMoveSpeed + Vector3.up * _rigidbody.velocity.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamageTest(_damageParametr);
            ReturObjectPool();
        }

        if (collision.collider.TryGetComponent(out Wall wall))
            ReturObjectPool();
    }

    public void Initialaze(Enemy target, float damage, float moveSpeedBoost, DamageParametr damageParametr)
    {
        _target = target;
        _damage = damage;
        _moveSpeedBoost = moveSpeedBoost;
        _damageParametr = damageParametr;

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(BackToPlayer());
    }

    private IEnumerator BackToPlayer()
    {
        while (Vector3.Magnitude(transform.position - _target.transform.position) >= 1f)
        {
            yield return null;
        }
    }
}