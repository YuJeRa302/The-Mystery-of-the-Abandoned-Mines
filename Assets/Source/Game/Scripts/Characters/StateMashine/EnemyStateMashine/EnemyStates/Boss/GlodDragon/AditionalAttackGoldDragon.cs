using System;

namespace Assets.Source.Game.Scripts.Characters
{
    public class AditionalAttackGoldDragon : BossAdditionalAttackState
    {
        private DragonSpell _spell;
        private float _damage;
        private EnemyAnimation _animationController;

        public AditionalAttackGoldDragon(StateMachine stateMachine, Player target, Enemy enemy)
            : base(stateMachine, target, enemy)
        {
            Target = target;
            Enemy = enemy;
            _animationController = Enemy.AnimationStateController;
            Boss boss = Enemy as Boss;
            GoldDragon goldDragon = boss as GoldDragon;
            _spell = goldDragon.DragonSpell;
            _damage = goldDragon.DamageSpell;

            _animationController.AdditionalAttacked += AditionalAttackAppalyDamage;
            _animationController.AnimationCompleted += OnAllowTransition;
        }

        public override void EnterState()
        {
            base.EnterState();
            AdditionalAttackEvent();
        }

        private void AditionalAttackAppalyDamage()
        {
            if (_spell.TryFindPlayer(out Player player))
                player.TakeDamage(Convert.ToInt32(_damage));
            else
                return;
        }
    }
}