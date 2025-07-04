using Assets.Source.Game.Scripts;

public class SpecialAttackGoldDragon : BossSpecialAttackState
{
    private int _countWave = 0;
    private int _maxCountWave = 2;

    public SpecialAttackGoldDragon(StateMashine stateMashine, Player player, Enemy enemy) : base(stateMashine, player, enemy)
    {
        Target = player;
        Enemy = enemy;
        AnimationController = Enemy.AnimationStateController;

        AnimationController.AnimationCompleted += SpetialAttackCounter;
    }

    public override void EnterState()
    {
        base.EnterState();
        _countWave = 0;
        GoldDragon dragon = Enemy as GoldDragon;
        dragon.DragonFlame.gameObject.SetActive(true);
        SpetiallAttackEvent();
    }

    public override void ExitState()
    {
        base.ExitState();
        GoldDragon dragon = Enemy as GoldDragon;
        dragon.DragonFlame.gameObject.SetActive(false);
    }

    private void SpetialAttackCounter()
    {
        _countWave++;

        if (_countWave >= _maxCountWave)
            CanTransit = true;
    }
}