using Assets.Source.Game.Scripts.Items;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Wapon", menuName = "Create Weapon/Paladin Weapon", order = 51)]
    public class PaladinWeaponData : WeaponData
    {
        [SerializeField] private WeaponPrefab _additionalWeapon;

        public WeaponPrefab AdditionalWeapon => _additionalWeapon;
    }
}