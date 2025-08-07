using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New GameConfig", menuName = "Create GameConfig", order = 51)]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levelDatas;
        [SerializeField] private List<ContractLevelData> _contractLevelDatas;
        [SerializeField] private List<WeaponData> _weaponDatas;
        [SerializeField] private List<PlayerClassData> _playerClassDatas;
        [SerializeField] private List<UpgradeData> _upgradeDatas;

        public PlayerClassData GetPlayerClassDataById(int id)
        {
            return GetData(_playerClassDatas, id);
        }

        public UpgradeData GetUpgradeDataById(int id)
        {
            return GetData(_upgradeDatas, id);
        }

        public WeaponData GetWeaponDataById(int id)
        {
            return GetData(_weaponDatas, id);
        }

        public LevelData GetLevelData(int id)
        {
            var levelData = GetData(_levelDatas, id);

            if (levelData != null)
                return levelData;

            return GetContractLevelData(id);
        }

        private LevelData GetContractLevelData(int id)
        {
            return GetData(_contractLevelDatas, id);
        }

        private T GetData<T>(List<T> data, int id)
            where T : IIdData
        {
            foreach (var d in data)
            {
                if (id == d.Id)
                    return d;
            }

            return default;
        }
    }
}