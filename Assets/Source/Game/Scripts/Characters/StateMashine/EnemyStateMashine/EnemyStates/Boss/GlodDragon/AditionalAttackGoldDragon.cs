using Assets.Source.Game.Scripts;
using System;

public class AditionalAttackGoldDragon : BossAdditionalAttackState
{
    private DragonSpell _spell;
    private float _damage;

    public AditionalAttackGoldDragon(StateMashine stateMashine, Player target, Enemy enemy) : base(stateMashine, target, enemy)
    {
        Target = target;
        Enemy = enemy;
        AnimationController = Enemy.AnimationStateController;
        Boss boss = Enemy as Boss;
        GoldDragon goldDragon = boss as GoldDragon;
        _spell = goldDragon.DragonSpell;
        _damage = goldDragon.DamageSpell;

        AnimationController.AdditionalAttacked += AditionalAttackAppalyDamage;
        AnimationController.AnimationCompleted += OnAllowTransition;
    }

    public override void EnterState()
    {
        base.EnterState();
        AdditionalAttackEvent();
    }

    protected override void AditionalAttackAppalyDamage()
    {
        if (_spell.TryFindPlayer(out Player player))
            player.TakeDamage(Convert.ToInt32(_damage));
        else
            return;
    }
}