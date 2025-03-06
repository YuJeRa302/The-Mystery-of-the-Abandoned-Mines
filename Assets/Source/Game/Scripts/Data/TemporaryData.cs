using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryData
{
    private LevelState[] _levelStates;
    private WeaponState[] _weaponStates;
    private int _countLevels;
    private int _upgradePoints;
    private int _playerScore;
    private string _language;
    private bool _muteStateSound;

    public TemporaryData(ConfigData configData) 
    {
        InitData(configData);
    }

    public int Coins { get; private set; }
    public float AmbientVolume { get; private set; }
    public float InterfaceVolume { get; private set; }
    public PlayerClassData PlayerClassData { get; private set; }
    public WeaponData WeaponData { get; private set; }
    public UpgradeState[] UpgradeStates { get; private set; }
    public ClassAbilityState[] ClassAbilityStates { get; private set; }
    public LevelData LevelData { get; private set; }
    public bool MuteStateSound => _muteStateSound;
    public int UpgradePoints => _upgradePoints;
    public int PlayerScore => _playerScore;
    public int CountLevels => _countLevels;
    public string Language => _language;

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

    public bool TryBuy(int value)
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

    public LevelState[] GetLevelStates() 
    {
        return _levelStates;
    }

    public void SetWeaponData(WeaponData weaponData) 
    {
        WeaponData = weaponData;
    }

    public void SetLevelData(LevelData levelData) 
    {
        LevelData = levelData;
    }

    public void SetPlayerClassData(PlayerClassData playerClassData) 
    {
        PlayerClassData = playerClassData;
    }

    public void SetUpgradePoints(int value)
    {
        _upgradePoints = value;
    }

    public void SetUpgradeState(List<UpgradeState> upgradeStates)
    {
        UpgradeStates = new UpgradeState[upgradeStates.Count];

        for (int index = 0; index < upgradeStates.Count; index++)
        {
            UpgradeStates[index] = upgradeStates[index];
        }
    }

    public void SetClassAbilityState(List<ClassAbilityState> classAbilityStates)
    {
        ClassAbilityStates = new ClassAbilityState[classAbilityStates.Count];

        for (int index = 0; index < classAbilityStates.Count; index++) 
        {
            ClassAbilityStates[index] = classAbilityStates[index];
        }
    }

    public void SetCoinsCount(int value) 
    {
        Coins = value;
    }

    public void SetCountLevels(int value)
    {
        _countLevels = value;
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
        _muteStateSound = value;
    }

    private void InitData(ConfigData configData) 
    {
        SetAmbientVolume(configData.AmbientVolume);
        SetInterfaceVolume(configData.SfxVolumeVolume);
        SetCurrentLanguage(configData.DefaultLanguage);
        SetUpgradePoints(configData.UpgradePoints);
        SetMuteStateSound(configData.IsMuted);
        SetCoinsCount(configData.Coins);
        SetClassAbilityState(configData.ClassAbilityStates);
        SetUpgradeState(configData.UpgradeStates);
        _levelStates = configData.DefaultLevelState;
        _weaponStates = configData.DefaultWeaponState;
    }
}