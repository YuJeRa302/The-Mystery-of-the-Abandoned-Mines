using Assets.Source.Game.Scripts;

public class SpecialAttackGoldDragon : BossSpecialAttackState
{
    private int _countWave = 0;
    private int _maxCountWave = 2;

    public SpecialAttackGoldDragon(StateMashine stateMashine, Player player, Enemy enemy) : base(stateMashine, player, enemy)
    {
        _target = player;
        _enemy = enemy;
        _animationController = _enemy.AnimationStateController;

        _animationController.AnimationCompleted += SpetialAttackCounter;
    }

    public override void EnterState()
    {
        base.EnterState();
        _countWave = 0;
        GoldDragon dragon = _enemy as GoldDragon;
        dragon.DragonFlame.gameObject.SetActive(true);
        SpetiallAttackEvent();
    }

    public override void ExitState()
    {
        base.ExitState();
        GoldDragon dragon = _enemy as GoldDragon;
        dragon.DragonFlame.gameObject.SetActive(false);
    }

    private void SpetialAttackCounter()
    {
        _countWave++;

        if (_countWave >= _maxCountWave)
            _canTransit = true;
    }
}