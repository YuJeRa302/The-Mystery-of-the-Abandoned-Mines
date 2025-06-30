using Assets.Source.Game.Scripts;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxemMssile : PoolObject
{
    [SerializeField] private Transform _viewConteiner;

    private Rigidbody _rigidbody;
    private WeponPrefab _weponPrefab;
    private Coroutine _coroutine;
    private Player _player;
    private Vector3 _direction;
    private float _moveSpeed = 2f;
    private float _moveSpeedBoost = 2f;
    private float _throwDuration;
    private bool _isReturn = false;
    private List<Enemy> _enemies = new List<Enemy>();

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

    public void Initialaze(Player player, DamageSource damageParametr, float moveSpeedBoost, float duration)
    {
        _player = player;
        _weponPrefab = _player.WeaponData.WeaponPrefab;
        _moveSpeedBoost = moveSpeedBoost;
        _throwDuration = duration - 2f;

        List<DamageParameter> damageSupportivePatametrs = new List<DamageParameter>(damageParametr.DamageParameters);

        for (int i = 0; i < damageSupportivePatametrs.Count; i++)
        {
            damageSupportivePatametrs[i] = new DamageParameter(damageParametr.DamageParameters[i].Value,
                damageParametr.DamageParameters[i].TypeDamageParameter);
        }

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

    public bool TryFindEnemys(out List<Enemy> enemies)
    {
        _enemies.Clear();
        enemies = new List<Enemy>();
        Collider[] coliderEnemy = Physics.OverlapSphere(transform.position, 2f);

        foreach (Collider collider in coliderEnemy)
        {
            if (collider.TryGetComponent(out Enemy enemy))
            {
                _enemies.Add(enemy);
            }
        }

        enemies.AddRange(_enemies);
        return enemies.Count > 0;
    }

    private IEnumerator Throw()
    {
        _isReturn = false;
        float time = 0;

        while (time < _throwDuration)
        {
            time += Time.deltaTime;
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

        ReturnObjectPool();
    }

    private void CorountineStart(IEnumerator corontine)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(corontine);
    }
}