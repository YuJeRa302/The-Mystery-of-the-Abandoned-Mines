using Assets.Source.Game.Scripts.SpawnersScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class BeholderAdditionalAttackState : BossAdditionalAttackState
    {
        private BulletSpawner _bulletSpawner;
        private Transform[] _shotPoints;
        private int _currentShotPointIndex;
        private int _maxShotPointIndex;
        private EnemyAnimation _animationController;

        public BeholderAdditionalAttackState(StateMachine stateMashine, Player target, Enemy enemy) : base(stateMashine, target, enemy)
        {
            Target = target;
            Enemy = enemy;
            _animationController = Enemy.AnimationStateController;
            Beholder boss = Enemy as Beholder;

            _shotPoints = boss.ShotPoints;
            _maxShotPointIndex = _shotPoints.Length;
            _currentShotPointIndex = 0;
            _bulletSpawner = new BulletSpawner(boss.Bullet, enemy.EnemyBulletPool, _shotPoints[_currentShotPointIndex], Enemy);

            _animationController.AdditionalAttacked += AditionalAttackAppalyDamage;
            _animationController.AnimationCompleted += OnAllowTransition;
        }

        public override void EnterState()
        {
            base.EnterState();
            _currentShotPointIndex = 0;
            AdditionalAttackEvent();
        }

        private void AditionalAttackAppalyDamage()
        {
            _bulletSpawner.SpawnBullet();
            _currentShotPointIndex++;

            if (_currentShotPointIndex == _maxShotPointIndex)
                return;

            _bulletSpawner.ChengeShotPoint(_shotPoints[_currentShotPointIndex]);
        }
    }
}