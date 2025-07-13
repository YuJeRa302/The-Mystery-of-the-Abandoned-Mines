using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Items;
using Assets.Source.Game.Scripts.PoolSystem;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AxeMissile : PoolObject
    {
        [SerializeField] private Transform _viewContainer;

        private Rigidbody _rigidbody;
        private WeaponPrefab _weaponPrefab;
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
            transform.DORotate(new Vector3(0, 360f, 0), 1f, 
                RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetRelative().SetEase(Ease.Linear);
        }

        private void FixedUpdate()
        {
            if (_isReturn)
            {
                transform.position = Vector3.Lerp(transform.position, _player.transform.position,
                    5f * _moveSpeedBoost * Time.fixedDeltaTime);
                _rigidbody.velocity = Vector3.zero;
                _direction = new Vector3(_player.transform.position.x,
                    _player.transform.position.y, _player.transform.position.z).normalized;
                _rigidbody.AddForce(_direction * _moveSpeed * _moveSpeedBoost);
                _direction = Vector3.zero;

                if (_rigidbody.velocity.y < 0f)
                    _rigidbody.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

                Vector3 horizontalVelocity = _rigidbody.velocity;
                horizontalVelocity.y = 0;

                if (horizontalVelocity.sqrMagnitude > _moveSpeed * _moveSpeed)
                    _rigidbody.velocity = horizontalVelocity.normalized * _moveSpeed +
                        Vector3.up * _rigidbody.velocity.y;
            }
        }

        public void Initialize(Player player, DamageSource damageParameter, float moveSpeedBoost, float duration)
        {
            _player = player;
            _weaponPrefab = _player.WeaponData.WeaponPrefab;
            _moveSpeedBoost = moveSpeedBoost;
            _throwDuration = duration - 2f;

            List<DamageParameter> damageSupportiveParameters = 
                new List<DamageParameter>(damageParameter.DamageParameters);

            for (int i = 0; i < damageSupportiveParameters.Count; i++)
            {
                damageSupportiveParameters[i] = new DamageParameter(damageParameter.DamageParameters[i].Value,
                    damageParameter.DamageParameters[i].TypeDamageParameter);
            }

            _weaponPrefab = Instantiate(_player.WeaponData.WeaponPrefab, transform);
            Vector3 rotate = _weaponPrefab.transform.eulerAngles;
            rotate.x = 90;
            _weaponPrefab.transform.rotation = Quaternion.Euler(rotate);
            CoroutineStart(Throw());
        }

        public void ThrowNow()
        {
            CoroutineStart(Throw());
        }

        public bool TryFindEnemies(out List<Enemy> enemies)
        {
            _enemies.Clear();
            enemies = new List<Enemy>();
            Collider[] colliderEnemy = Physics.OverlapSphere(transform.position, 2f);

            foreach (Collider collider in colliderEnemy)
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

            CoroutineStart(BackToPlayer());
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

        private void CoroutineStart(IEnumerator corontine)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(corontine);
        }
    }
}