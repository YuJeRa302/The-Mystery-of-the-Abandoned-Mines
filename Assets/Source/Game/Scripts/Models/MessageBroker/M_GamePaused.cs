namespace Assets.Source.Game.Scripts.Models
{
    public struct M_GamePaused
    {
        private readonly bool _isGamePaused;

        public M_GamePaused(bool state)
        {
            _isGamePaused = state;
        }

        public readonly bool IsGamePaused => _isGamePaused;
    }
}