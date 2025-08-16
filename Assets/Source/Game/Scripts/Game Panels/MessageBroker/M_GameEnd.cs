namespace Assets.Source.Game.Scripts.GamePanels
{
    public struct M_GameEnd
    {
        private bool _state;

        public M_GameEnd(bool state)
        {
            _state = state;
        }

        public bool State => _state;
    }
}