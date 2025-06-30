using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;

[Serializable]
public class UpgradeService
{
    public List<UpgradeState> UpgradeStates { get; private set; } = new ();

    public void SetUpgradeStates(List<UpgradeState> upgradeStates)
    {
        UpgradeStates = upgradeStates;
    }

    public void SetUpgradeState(UpgradeState newUpgradeState) 
    {
        if (UpgradeStates != null)
        {
            for (int index = 0; index < UpgradeStates.Count; index++) 
            {
                if (UpgradeStates[index].Id == newUpgradeState.Id)
                    UpgradeStates[index].CurrentLevel = newUpgradeState.CurrentLevel;
                else
                    UpgradeStates.Add(new(newUpgradeState.Id, newUpgradeState.CurrentLevel));
            }
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
        if (UpgradeStates != null)
        {
            foreach (UpgradeState upgradeState in UpgradeStates)
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
        UpgradeStates.Add(upgradeState);
        return upgradeState;
    }
}
