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
        Target = target;
        Enemy = enemy;
        AnimationController = Enemy.AnimationStateController;
        Beholder boss = Enemy as Beholder;

        _shotPoints = boss.ShotPoints;
        _maxShotPointIndex = _shotPoints.Length;
        _currentShotPointIndex = 0;
        _bulletSpawner = new BulletSpawner(boss.Bullet, boss.Pool, _shotPoints[_currentShotPointIndex], Enemy);

        AnimationController.AdditionalAttacked += AditionalAttackAppalyDamage;
        AnimationController.AnimationCompleted += OnAllowTransition;
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
        _currentShotPointIndex++;

        if (_currentShotPointIndex == _maxShotPointIndex)
            return;

        _bulletSpawner.ChengeShotPoint(_shotPoints[_currentShotPointIndex]);
    }
}