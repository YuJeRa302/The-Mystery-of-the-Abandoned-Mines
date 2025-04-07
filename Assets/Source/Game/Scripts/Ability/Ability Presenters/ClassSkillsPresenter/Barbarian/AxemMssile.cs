using Assets.Source.Game.Scripts;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AxemMssile : PoolObject
{
    [SerializeField] private Transform _viewConteiner;

    private Rigidbody _rigidbody;
    private WeponPrefab _weponPrefab;
    private Coroutine _coroutine;
    private Player _player;
    private Vector3 _direction;
    private DamageParametr _damage;
    private float _moveSpeed = 2f;
    private float _moveSpeedBoost = 2f;
    private bool _isReturn = false;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        transform.DORotate(new Vector3(0, 360f, 0), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetRelative().SetEase(Ease.Linear);
    }

    private void FixedUpdate()
    {
        if (_isReturn)
        {
            transform.position = Vector3.Lerp(transform.position, _player.transform.position, 5f * _moveSpeedBoost * Time.fixedDeltaTime);
                _rigidbody.velocity = Vector3.zero;
                _direction = new Vector3(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z).normalized;
                _rigidbody.AddForce(_direction * _moveSpeed * _moveSpeedBoost);
                _direction = Vector3.zero;

                if (_rigidbody.velocity.y < 0f)
                    _rigidbody.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

                Vector3 horizontalVelocity = _rigidbody.velocity;
                horizontalVelocity.y = 0;

                if (horizontalVelocity.sqrMagnitude > _moveSpeed * _moveSpeed)
                    _rigidbody.velocity = horizontalVelocity.normalized * _moveSpeed + Vector3.up * _rigidbody.velocity.y;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Enemy enemy))
        {
            Debug.Log("Axe");
            Debug.Log(_damage.DamageSupportivePatametrs.Count);
            enemy.TakeDamageTest(_damage);
            Vector3 direction = (enemy.transform.position - transform.position) * 5;
        }
    }

    public void Initialaze(Player player, DamageParametr damageParametr, float moveSpeedBoost)
    {
        _player = player;
        _weponPrefab = _player.WeaponData.WeaponPrefab;
        _moveSpeedBoost = moveSpeedBoost;
        _damage = damageParametr;

        _weponPrefab = Instantiate(_player.WeaponData.WeaponPrefab, transform);
        Vector3 rotate = _weponPrefab.transform.eulerAngles;
        rotate.x = 90;
        _weponPrefab.transform.rotation = Quaternion.Euler(rotate);
        CorountineStart(Throw());
    }

    public void ThrowNow()
    {
        CorountineStart(Throw());
    }

    private IEnumerator Throw()
    {
        _isReturn = false;
        float time = 1.5f;

        while (time >= 0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        CorountineStart(BackToPlayer());
    }

    private IEnumerator BackToPlayer()
    {
        _isReturn = true;

        while (Vector3.Magnitude(transform.position - _player.transform.position) >= 1f)
        {
            yield return null;
        }

        ReturObjectPool();
    }

    private void CorountineStart(IEnumerator corontine)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(corontine);
    }
}