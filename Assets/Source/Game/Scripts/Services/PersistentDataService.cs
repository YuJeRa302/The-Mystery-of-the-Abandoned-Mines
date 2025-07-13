using Assets.Source.Game.Scripts.Saves;

namespace Assets.Source.Game.Scripts.Services
{
    public class PersistentDataService
    {
        public PlayerProgress PlayerProgress;

        public PersistentDataService()
        {
            if (PlayerProgress == null)
                PlayerProgress = new();
        }

        public bool TrySpendCoins(int value)
        {
            if (PlayerProgress.Coins >= value)
            {
                PlayerProgress.Coins -= value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TrySpendUpgradePoints(int value)
        {
            if (PlayerProgress.UpgradePoints >= value)
            {
                PlayerProgress.UpgradePoints -= value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}