using Assets.Source.Game.Scripts;

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

    public override void UpdateState()
    {
        base.UpdateState();
    }

    private void LaunchBullet()
    {
        _bulletSpawner.SpawnBullet();
    }
}