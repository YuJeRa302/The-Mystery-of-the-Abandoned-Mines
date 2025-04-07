using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Contarct Data", menuName = "Create Contarct Data", order = 51)]
    public class ContractLevelData : LevelData
    {
        [SerializeField] private WeaponData[] _weaponDatas;

        public WeaponData[] WeaponDatas => _weaponDatas;
    }
}