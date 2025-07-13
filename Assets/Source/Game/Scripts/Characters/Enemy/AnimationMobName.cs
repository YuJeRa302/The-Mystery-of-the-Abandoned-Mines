namespace Assets.Source.Game.Scripts.Characters
{
    public class AnimationMobName
    {
        private const string _moveAnimation = "Move";
        private const string _attackAnimation = "Attack";
        private const string _additionalAttackAnimation = "AdditionalAttack";
        private const string _specialAttackAnimation = "SpecialAttack";
        private const string _idleAnimation = "Idle";

        public string MoveAnimation => _moveAnimation;
        public string AttackAnimation => _attackAnimation;
        public string AdditionalAttackAnimation => _additionalAttackAnimation;
        public string SpecialAttackAnimation => _specialAttackAnimation;
        public string IdleAnimation => _idleAnimation;
    }
}