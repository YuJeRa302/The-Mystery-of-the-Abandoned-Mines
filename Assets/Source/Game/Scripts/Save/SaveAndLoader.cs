using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class SaveAndLoader : ISaveAndLoadProgress
{
    private readonly PersistentDataService _persistentDataService;
    private readonly ConfigData _configData;
    private readonly int _defaultIntValue = 0;

    private TemporaryData _temporaryData;

    public SaveAndLoader(PersistentDataService persistentDataService, ConfigData configData)
    {
        _persistentDataService = persistentDataService;
        _configData = configData;
    }

    public SaveAndLoader(PersistentDataService persistentDataService)
    {
        _persistentDataService = persistentDataService;
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

    public void LoadDataFromCloud()
    {
        _persistentDataService.PlayerProgress.AmbientVolume = YG2.saves.AmbientVolume;
        _persistentDataService.PlayerProgress.SfxVolume = YG2.saves.SfxVolumeVolume;
        _persistentDataService.PlayerProgress.Score = YG2.saves.Score;
        _persistentDataService.PlayerProgress.Coins = YG2.saves.Coins;
        _persistentDataService.PlayerProgress.IsMuted = YG2.saves.IsMuted;
        _persistentDataService.PlayerProgress.Language = YG2.saves.DefaultLanguage;
        _persistentDataService.PlayerProgress.UpgradePoints = YG2.saves.UpgradePoints;
        _persistentDataService.PlayerProgress.LevelService.SetLevelStates(YG2.saves.DefaultLevelState);
        _persistentDataService.PlayerProgress.WeaponService.SetWeaponStates(YG2.saves.DefaultWeaponState);
        _persistentDataService.PlayerProgress.ClassAbilityService.SetClassAbilityStates(YG2.saves.ClassAbilityStates);
        _persistentDataService.PlayerProgress.UpgradeService.SetUpgradeStates(YG2.saves.UpgradeStates);
    }

    public void LoadDataFromConfig()
    {
        _persistentDataService.PlayerProgress.AmbientVolume = _configData.AmbientVolume;
        _persistentDataService.PlayerProgress.SfxVolume = _configData.SfxVolumeVolume;
        _persistentDataService.PlayerProgress.Score = _defaultIntValue;
        _persistentDataService.PlayerProgress.Coins = _configData.Coins;
        _persistentDataService.PlayerProgress.IsMuted = _configData.IsMuted;
        _persistentDataService.PlayerProgress.Language = _configData.DefaultLanguage;
        _persistentDataService.PlayerProgress.UpgradePoints = _configData.UpgradePoints;
        _persistentDataService.PlayerProgress.LevelService.SetLevelStates(_configData.DefaultLevelState);
        _persistentDataService.PlayerProgress.WeaponService.SetWeaponStates(_configData.DefaultWeaponState);
        _persistentDataService.PlayerProgress.ClassAbilityService.SetClassAbilityStates(_configData.ClassAbilityStates);
        _persistentDataService.PlayerProgress.UpgradeService.SetUpgradeStates(_configData.UpgradeStates);
    }

    public void LoadDataFromPrefs()
    {
        if (PlayerPrefs.HasKey(GameConstants.PlayerProgressDataKey))
        {
            var jsonString = PlayerPrefs.GetString(GameConstants.PlayerProgressDataKey);
            JsonUtility.FromJsonOverwrite(jsonString, _persistentDataService.PlayerProgress);
        }
    }

    public void SaveGameProgerss(int score, int coins, int upgradePoints, int levelId, bool isComplete, bool isGameInterrupted)
    {
        if (isGameInterrupted)
            return;

        _persistentDataService.PlayerProgress.Score += score;
        _persistentDataService.PlayerProgress.Coins += coins;
        _persistentDataService.PlayerProgress.UpgradePoints += upgradePoints;
        _persistentDataService.PlayerProgress.LevelService.UpdateLevelStates(levelId, isComplete);
        SaveData();
    }

    public void SaveDataToPrefs()
    {
        var jsonString = JsonUtility.ToJson(_persistentDataService.PlayerProgress);
        PlayerPrefs.SetString(GameConstants.PlayerProgressDataKey, jsonString);
        PlayerPrefs.Save();
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
            UpgradeStates = new List<UpgradeState>(_temporaryData.UpgradeStates),
            ClassAbilityStates = new List<ClassAbilityState>(_temporaryData.ClassAbilityStates),
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