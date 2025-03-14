using Assets.Source.Game.Scripts;
using System.Collections;
using UnityEngine;

public class Summon : PoolObject
{
    private readonly float _distanceToTarget = 2f;
    private readonly float _searchRadius = 10f;

    [SerializeField] private SummonStateMashineExample _mashineExample;
    [SerializeField] private SummonAnimation _animation;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private DamageParametr _damage;

    private Player _player;
    private Enemy _target;
    private Coroutine _coroutine;
    private float _lifeTime;
    private float _maxHealth;
    private float _attackDelay = 2f;

    public Enemy Target => _target;
    public SummonAnimation Animation => _animation;
    public float DistanceToTarget => _distanceToTarget;
    public float SearchRadius => _searchRadius;
    public float AttackDelay => _attackDelay;
    public DamageParametr Damage => _damage;
    public float LifeTime => _lifeTime;
    public float MoveSpeed => _moveSpeed;

    public void Initialize(Player player, DamageParametr damageParametr, float lifeTime)
    {
        _player = player;
        _damage = _player.PlayerAttacker.DamageParametr;

        foreach (var supportivePatametr in damageParametr.DamageSupportivePatametrs)
        {
            if (supportivePatametr.SupportivePatametr == TypeSupportivePatametr.Damage)
            {
                foreach (var mainDamge in _damage.DamageSupportivePatametrs)
                {
                    if (mainDamge.SupportivePatametr == TypeSupportivePatametr.Damage)
                    {
                        //mainDamge.Value = supportivePatametr.Value;
                        Debug.Log(mainDamge.Value);
                    }
                }
            }
        }

        _lifeTime = lifeTime;
        _mashineExample.MashineInitialized += OnStateMashineInitialize;
        _mashineExample.InitializeStateMashine(_player);

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(CountLifeTime());
    }

    public void SetTarget(Enemy target)
    {
        _target = target;
    }

    public void DisableTarget()
    {
        _target = null;
    }

    public void ResetStats()
    {
        _mashineExample.ResetState();

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(CountLifeTime());
    }

    private void OnStateMashineInitialize()
    {
        _animation.Initialization(_mashineExample);
    }

    private IEnumerator CountLifeTime()
    {
        yield return new WaitForSeconds(_lifeTime);
        ReturnToPool();
    }
}