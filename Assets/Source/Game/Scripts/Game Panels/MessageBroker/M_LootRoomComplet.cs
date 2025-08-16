namespace Assets.Source.Game.Scripts.GamePanels
{
    public struct M_LootRoomComplet
    {
        private int _loot;

        public M_LootRoomComplet(int reward)
        {
            _loot = reward;
        }

        public int Reward => _loot;
    }
}