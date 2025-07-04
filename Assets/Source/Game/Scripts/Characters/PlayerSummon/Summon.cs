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
    [SerializeField] private DamageSource _damageSource;

    private Player _player;
    private Enemy _target;
    private Coroutine _coroutine;
    private float _lifeTime;
    private float _attackDelay = 2f;

    public Enemy Target => _target;
    public SummonAnimation Animation => _animation;
    public float DistanceToTarget => _distanceToTarget;
    public float SearchRadius => _searchRadius;
    public float AttackDelay => _attackDelay;
    public DamageSource DamageSource => _damageSource;
    public float MoveSpeed => _moveSpeed;

    public void Initialize(Player player, DamageSource damageSource, float lifeTime)
    {
        _player = player;
        _damageSource = damageSource;
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