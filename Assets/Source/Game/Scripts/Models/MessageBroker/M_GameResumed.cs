namespace Assets.Source.Game.Scripts.Models
{
    public struct M_GameResumed
    {
        private readonly bool _isGameResumed;

        public M_GameResumed(bool state)
        {
            _isGameResumed = state;
        }

        public readonly bool IsGameResumed => _isGameResumed;
    }
}