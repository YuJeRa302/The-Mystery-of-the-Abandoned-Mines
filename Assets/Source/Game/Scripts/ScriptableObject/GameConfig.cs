using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
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
            foreach (PlayerClassData playerClassData in _playerClassDatas)
            {
                if (id == playerClassData.Id)
                    return playerClassData;
            }

            return null;
        }

        public UpgradeData GetUpgradeDataById(int id)
        {
            foreach (UpgradeData upgradeData in _upgradeDatas)
            {
                if (id == upgradeData.Id)
                    return upgradeData;
            }

            return null;
        }

        public WeaponData GetWeaponDataById(int id)
        {
            foreach (WeaponData weaponData in _weaponDatas)
            {
                if (id == weaponData.Id)
                    return weaponData;
            }

            return null;
        }

        public LevelData GetLevelData(int id)
        {
            foreach (LevelData levelData in _levelDatas)
            {
                if (id == levelData.Id)
                    return levelData;
            }

            return GetContractLevelData(id);
        }

        private LevelData GetContractLevelData(int id)
        {
            foreach (LevelData levelData in _contractLevelDatas)
            {
                if (id == levelData.Id)
                    return levelData;
            }

            return null;
        }
    }
}