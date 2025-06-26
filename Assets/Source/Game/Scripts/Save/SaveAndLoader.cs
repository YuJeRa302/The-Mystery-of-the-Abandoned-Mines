using Assets.Source.Game.Scripts;
using System;
using UnityEngine;
using YG;

public class SaveAndLoader : ISaveAndLoadProgress
{
    private TemporaryData _temporaryData;

    public SaveAndLoader()
    {
    }

    public void Initialize(TemporaryData temporaryData)
    {
        _temporaryData = temporaryData;
        _temporaryData.ChangedData += SaveData;
    }

    public bool TryGetGameData()
    {
        return YG2.saves.HasSave;
    }

    public void SaveData()
    {
        var newSaveData = new SavesYG
        {
            Coins = _temporaryData.Coins,
            AmbientVolume = _temporaryData.AmbientVolume,
            SfxVolumeVolume = _temporaryData.InterfaceVolume,
            IsMuted = _temporaryData.MuteStateSound,
            UpgradePoints = _temporaryData.UpgradePoints,
            UpgradeStates = new System.Collections.Generic.List<UpgradeState>(_temporaryData.UpgradeStates),
            ClassAbilityStates = new System.Collections.Generic.List<ClassAbilityState>(_temporaryData.ClassAbilityStates),
            DefaultWeaponState = new WeaponState[_temporaryData.WeaponStates.Length],
            DefaultLevelState = new LevelState[_temporaryData.LevelStates.Length],
            Score = _temporaryData.PlayerScore,
            HasSave = true
        };

        Array.Copy(_temporaryData.WeaponStates, newSaveData.DefaultWeaponState, newSaveData.DefaultWeaponState.Length);
        Array.Copy(_temporaryData.LevelStates, newSaveData.DefaultLevelState, newSaveData.DefaultLevelState.Length);

        string oldDataJson = JsonUtility.ToJson(YG2.saves);
        string newDatatJson = JsonUtility.ToJson(newSaveData);

        if (oldDataJson != newDatatJson)
        {
            YG2.saves = newSaveData;
            YG2.SaveProgress();
        }
    }
}