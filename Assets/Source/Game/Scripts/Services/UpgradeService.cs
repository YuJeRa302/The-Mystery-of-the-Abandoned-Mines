using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Upgrades;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Services
{
    [Serializable]
    public class UpgradeService
    {
        [SerializeField] private List<UpgradeState> _upgradeStates = new();

        public List<UpgradeState> UpgradeStates => _upgradeStates;

        public void SetUpgradeStates(List<UpgradeState> upgradeStates)
        {
            _upgradeStates = upgradeStates;
        }

        public void SetUpgradeState(UpgradeState newUpgradeState)
        {
            if (_upgradeStates != null)
            {
                UpgradeState upgradeState = _upgradeStates.Find(state => state.Id == newUpgradeState.Id);

                if (upgradeState != null)
                    upgradeState.ChangeCurrentLevel(newUpgradeState.CurrentLevel);
                else
                    _upgradeStates.Add(new(newUpgradeState.Id, newUpgradeState.CurrentLevel));
            }
        }

        public UpgradeState GetUpgradeState(UpgradeData upgradeData)
        {
            UpgradeState upgradeState = FindUpgradeState(upgradeData.Id);

            if (upgradeState == null)
                upgradeState = InitState(upgradeData);

            return upgradeState;
        }

        private UpgradeState FindUpgradeState(int id)
        {
            if (_upgradeStates != null)
            {
                foreach (UpgradeState upgradeState in _upgradeStates)
                {
                    if (upgradeState.Id == id)
                        return upgradeState;
                }
            }

            return null;
        }

        private UpgradeState InitState(UpgradeData upgradeData)
        {
            UpgradeState upgradeState = new(upgradeData.Id, 0);
            _upgradeStates.Add(upgradeState);
            return upgradeState;
        }
    }
}