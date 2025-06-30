namespace Assets.Source.Game.Scripts
{
    [System.Serializable]
    public class WeaponState
    {
        public int Id;
        public bool IsEquip;
        public bool IsUnlock;

        public WeaponState(int id, bool isEquip, bool isUnlock) 
        {
            Id = id;
            IsEquip = isEquip;
            IsUnlock = isUnlock;
        }
    }
}