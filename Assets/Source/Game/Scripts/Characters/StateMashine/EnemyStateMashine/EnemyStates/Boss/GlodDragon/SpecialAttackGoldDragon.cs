namespace Assets.Source.Game.Scripts.Characters
{
    public class SpecialAttackGoldDragon : BossSpecialAttackState
    {
        private int _countWave = 0;
        private int _maxCountWave = 2;

        public SpecialAttackGoldDragon(StateMachine stateMashine, Enemy enemy) : base(stateMashine, enemy)
        {
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
}