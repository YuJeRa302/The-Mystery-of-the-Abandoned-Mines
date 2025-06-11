using Assets.Source.Game.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class TemporaryData
{
    private readonly int _shiftIndex = 1;

    private LevelState[] _levelStates;
    private WeaponState[] _weaponStates;
    private int _countLevels;
    private int _playerScore;
    private string _language;

    public event Action ChangedData;

    public TemporaryData(ConfigData configData) 
    {
        InitData(configData);
    }

    public TemporaryData(SavesYG savesYG)
    {
        InitData(savesYG);
    }

    public bool IsGamePause { get; private set; } = false;
    public int Coins { get; private set; }
    public float AmbientVolume { get; private set; }
    public float InterfaceVolume { get; private set; }
    public PlayerClassData PlayerClassData { get; private set; }
    public WeaponData WeaponData { get; private set; }
    public UpgradeState[] UpgradeStates { get; private set; }
    public ClassAbilityState[] ClassAbilityStates { get; private set; }
    public LevelData LevelData { get; private set; }
    public LevelState CurrentLevelState { get; private set; }
    public bool MuteStateSound { get; private set; } = false;
    public int UpgradePoints { get; private set; }
    public int PlayerScore => _playerScore;
    public int CountLevels => _countLevels;
    public string Language => _language;
    public WeaponState[] WeaponStates => _weaponStates;
    public LevelState[] LevelStates => _levelStates;
    public UpgradeData[] UpgradeDatas { get; private set; }

    public void SaveProgress(Player player, bool IsComplete, bool isGameInterrupted)
    {
        Debug.Log($"complite - {IsComplete}/n interruped - {isGameInterrupted}");
        if (isGameInterrupted)
            return;
        Debug.Log(player.Score);
        _playerScore += player.Score;
        Coins += player.Coins;
        UpgradePoints += player.UpgradePoints;

        Debug.Log("Coins !!!!!!!!! - " + Coins);
        Debug.Log("player.Score !!!!!!!!! - " + player.Score);

        CurrentLevelState = new LevelState()
        {
            Id = LevelData.Id,
            IsComplete = IsComplete,
            CurrentCompleteStages = LevelData.CountStages,
            Tier = LevelData.Tier
        };

        Debug.Log("IsComplete !!!!!!!!! - " + CurrentLevelState.IsComplete);

        if (CurrentLevelState.IsComplete != false)
            UpdateLevelStates(CurrentLevelState);

        ChangedData?.Invoke();
    }

    public ClassAbilityState GetClassAbilityState(int id)
    {
        if (ClassAbilityStates != null)
        {
            foreach (ClassAbilityState classAbilityState in ClassAbilityStates)
            {
                if (classAbilityState.Id == id)
                    return classAbilityState;
            }
        }

        return null;
    }

    public UpgradeState GetUpgradeState(int id)
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

    public WeaponState GetWeaponState(int id)
    {
        if (_weaponStates != null)
        {
            foreach (WeaponState weaponState in _weaponStates)
            {
                if (weaponState.Id == id)
                    return weaponState;
            }
        }

        return null;
    }

    public LevelState GetLevelState(int id)
    {
        if (_levelStates != null)
        {
            foreach (LevelState levelState in _levelStates)
            {
                if (levelState.Id == id)
                    return levelState;
            }
        }

        return null;
    }

    public void AddCoins(int value)
    {
        Coins += value;
    }

    public void AddUpgradePoints(int value)
    {
        UpgradePoints += value;
    }

    public bool TrySpendCoins(int value)
    {
        if (Coins >= value) 
        {
            Coins -= value;
            return true;
        }
        else 
        {
            return false;
        }
    }

    public bool TrySpendUpgradePoints(int value)
    {
        if (UpgradePoints >= value)
        {
            UpgradePoints -= value;
            return true;
        }
        else
        {
            return false;
        }
    }

    public LevelState[] GetLevelStates() 
    {
        return _levelStates;
    }

    public void SetPauseGame(bool state)
    {
        IsGamePause = state;
    }

    public void SetUpgradesData(List<UpgradeData> upgradeDatas)
    {
        UpgradeDatas = upgradeDatas.ToArray();
    }

    public void SetWeaponData(WeaponData weaponData) 
    {
        WeaponData = weaponData;
    }

    public void SetLevelData(LevelData levelData) 
    {
        LevelData = levelData;

        foreach (var state  in LevelStates)
        {
            if (state.Id == LevelData.Id) 
            {
                CurrentLevelState = new LevelState()
                {
                    Id = state.Id,
                    IsComplete = state.IsComplete,
                    CurrentCompleteStages = state.CurrentCompleteStages,
                    Tier = state.Tier
                };
            }
        }
    }

    public void SetPlayerClassData(PlayerClassData playerClassData) 
    {
        PlayerClassData = playerClassData;
    }

    public void SetUpgradePoints(int value)
    {
        UpgradePoints = value;
    }

    public void SetUpgradeState(List<UpgradeState> upgradeStates)
    {
        UpgradeStates = new UpgradeState[upgradeStates.Count];

        for (int index = 0; index < upgradeStates.Count; index++)
        {
            UpgradeStates[index] = new UpgradeState(upgradeStates[index].Id, upgradeStates[index].CurrentLevel);
        }

        ChangedData?.Invoke();
    }

    public void SetClassAbilityState(List<ClassAbilityState> classAbilityStates)
    {
        ClassAbilityStates = new ClassAbilityState[classAbilityStates.Count];

        for (int index = 0; index < classAbilityStates.Count; index++) 
        {
            ClassAbilityStates[index] = new ClassAbilityState(classAbilityStates[index].Id, classAbilityStates[index].CurrentLevel);
        }

        ChangedData?.Invoke();
    }

    public void UpdateLevelStates(LevelState currentLevelState)
    {
        for (int index = 0; index < _levelStates.Length; index++)
        {
            if (_levelStates[index].Id == currentLevelState.Id)
                _levelStates[index].IsComplete = currentLevelState.IsComplete;
        }
    }

    public void UpdateWeaponStates(WeaponState weaponState) 
    {
        if (weaponState == null)
            return;

        int newArrayLength = _weaponStates.Length;
        newArrayLength++;
        WeaponState[] tempWeaponStates = new WeaponState[newArrayLength];

        for (int index = 0; index < _weaponStates.Length; index++) 
        {
            tempWeaponStates[index] = _weaponStates[index];
        }

        tempWeaponStates[tempWeaponStates.Length - _shiftIndex] = weaponState;
        _weaponStates = new WeaponState[tempWeaponStates.Length];

        for (int index = 0; index < tempWeaponStates.Length; index++)
        {
            _weaponStates[index] = tempWeaponStates[index];
        }

        ChangedData?.Invoke();
    }

    public void SetCoinsCount(int value) 
    {
        Coins = value;
        ChangedData?.Invoke();
    }

    public void SetCountLevels(int value)
    {
        _countLevels = value;
        ChangedData?.Invoke();
    }

    public void SetCurrentLanguage(string value)
    {
        _language = value;
    }

    public void SetAmbientVolume(float value)
    {
        AmbientVolume = value;
    }

    public void SetInterfaceVolume(float value)
    {
        InterfaceVolume = value;
    }

    public void SetMuteStateSound(bool value) 
    {
        MuteStateSound = value;
        ChangedData?.Invoke();
    }

    private void SetLanguage()
    {
#if UNITY_EDITOR
        SetCurrentLanguage("ru");
#else
        string languageCode = YG2.envir.browserLang;
        SetCurrentLanguage(languageCode);
#endif
    }

    private void InitData(ConfigData configData) 
    {
        SetAmbientVolume(configData.AmbientVolume);
        SetInterfaceVolume(configData.SfxVolumeVolume);
        SetLanguage();
        SetUpgradePoints(configData.UpgradePoints);
        SetMuteStateSound(configData.IsMuted);
        SetCoinsCount(configData.Coins);
        SetClassAbilityState(configData.ClassAbilityStates);
        SetUpgradeState(configData.UpgradeStates);
        _levelStates = configData.DefaultLevelState;
        _weaponStates = configData.DefaultWeaponState;
        ChangedData?.Invoke();
    }

    private void InitData(SavesYG savesYG)
    {
        SetAmbientVolume(savesYG.AmbientVolume);
        SetInterfaceVolume(savesYG.SfxVolumeVolume);
        SetCurrentLanguage(savesYG.DefaultLanguage);
        SetUpgradePoints(savesYG.UpgradePoints);
        SetMuteStateSound(savesYG.IsMuted);
        SetCoinsCount(savesYG.Coins);
        SetClassAbilityState(savesYG.ClassAbilityStates);
        SetUpgradeState(savesYG.UpgradeStates);
        _levelStates = savesYG.DefaultLevelState;
        _weaponStates = savesYG.DefaultWeaponState;
        ChangedData?.Invoke();
    }
}