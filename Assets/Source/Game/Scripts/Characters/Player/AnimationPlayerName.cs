namespace Assets.Source.Game.Scripts.Characters
{
    public class AnimationPlayerName
    {
        private const string _moveAnimation = "Speed";
        private const string _attackAnimation = "Attack";
        private const string _coverAnimation = "Cover";
        private const string _endAnimation = "EndUsed";

        public string MoveAnimation => _moveAnimation;
        public string AttackAnimation => _attackAnimation;
        public string CoverAnimation => _coverAnimation;
        public string EndAnimation => _endAnimation;
    }
}