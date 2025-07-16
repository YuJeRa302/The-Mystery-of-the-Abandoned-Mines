using UnityEngine;

namespace Assets.Source.Game.Scripts.Items
{
    [System.Serializable]
    public class WeaponState
    {
        [SerializeField] private int _id;
        [SerializeField] private bool _isEquip;
        [SerializeField] private bool _isUnlock;

        public WeaponState(int id, bool isEquip, bool isUnlock)
        {
            _id = id;
            _isEquip = isEquip;
            _isUnlock = isUnlock;
        }

        public int Id => _id;
        public bool IsEquip => _isEquip;
        public bool IsUnlock => _isUnlock;

        public void UnlockWeapon() => _isUnlock = true;
    }
}