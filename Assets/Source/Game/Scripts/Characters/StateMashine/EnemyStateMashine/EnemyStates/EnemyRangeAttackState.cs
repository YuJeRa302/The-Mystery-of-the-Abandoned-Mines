using Assets.Source.Game.Scripts;
using UnityEngine;

public class EnemyRangeAttackState : EnemyAttackState
{
    private BulletSpawner _bulletSpawner;

    public EnemyRangeAttackState(StateMashine stateMashine, Player target, Enemy enemy, BulletSpawner bulletSpawner) : base(stateMashine, target, enemy)
    {
        _enemy = enemy;
        _target = target;
        _attackRange = _enemy.AttackDistance;
        _damage = _enemy.Damage;
        _attackDelay = _enemy.AttackDelay;
        _animationController = _enemy.AnimationStateController;
        _bulletSpawner = bulletSpawner;
        _animationController.Attacked += LaunchBullet;
    }

    public override void SubscrabeIvent()
    {
        _animationController.Attacked += LaunchBullet;
    }

    public override void UpdateState()
    {
        if (_canTransit)
        {
            _directionToTarget = _enemy.transform.position - _target.transform.position;
            _distanceToTarget = _directionToTarget.magnitude;

            if (_distanceToTarget > _attackRange)
                _stateMashine.SetState<EnemyMoveState>();

            if (Attack())
            {
                AttackEvent();
            }
        }
    }

    protected override bool Attack()
    {
        if (_distanceToTarget <= _attackRange)
        {
            _enemy.transform.LookAt(_target.transform.position);

            if (_lastAttackTime <= 0)
            {
                _lastAttackTime = _attackDelay;
                _canTransit = false;
                return true;
            }
        }

        _lastAttackTime -= Time.deltaTime;
        return false;
    }

    private void LaunchBullet()
    {
        _bulletSpawner.SpawnBullet();
        _canTransit = true;
    }
}