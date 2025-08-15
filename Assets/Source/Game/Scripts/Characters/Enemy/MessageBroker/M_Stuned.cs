namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_Stuned
    {
        private bool _isStun;

        public M_Stuned(bool isStun)
        {
            _isStun = isStun;
        }

        public bool IsStun => _isStun;
    }
}