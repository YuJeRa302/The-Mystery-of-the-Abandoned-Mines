using Assets.Source.Game.Scripts;
using UnityEngine;

public class BeholderAdditionalAttackState : BossAdditionalAttackState
{
    private BulletSpawner _bulletSpawner;
    private Transform[] _shotPoints;
    private int _currentShotPointIndex;
    private int _maxShotPointIndex;

    public BeholderAdditionalAttackState(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine, target, enemy)
    {
        _target = target;
        _enemy = enemy;
        _animationController = _enemy.AnimationStateController;
        Beholder boss = _enemy as Beholder;

        _shotPoints = boss.ShotPoints;
        _maxShotPointIndex = _shotPoints.Length;
        _currentShotPointIndex = 0;
        _bulletSpawner = new BulletSpawner(boss.Bullet, boss.Pool, _shotPoints[_currentShotPointIndex], boss.Damage);

        _animationController.AdditionalAttacked += AditionalAttackAppalyDamage;
        _animationController.AnimationCompleted += OnAllowTransition;
    }

    public override void EnterState()
    {
        base.EnterState();
        _currentShotPointIndex = 0;
        AdditionalAttackEvent();
    }

    protected override void AditionalAttackAppalyDamage()
    {
        _bulletSpawner.SpawnBullet();
        Debug.Log("SPAWNBULLET");
        _currentShotPointIndex++;

        if (_currentShotPointIndex == _maxShotPointIndex)
            return;

        _bulletSpawner.ChengeShotPoint(_shotPoints[_currentShotPointIndex]);
    }
}