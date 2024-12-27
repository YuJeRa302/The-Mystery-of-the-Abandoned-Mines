using Assets.Source.Game.Scripts;

public class TemporaryData
{
    private UpgradeState[] _upgradeState;
    private LevelState[] _levelStates;
    private WeaponState[] _weaponStates;
    private int _countLevels;
    private int _upgradePoints;
    private int _playerScore;
    private string _language;
    private float _interfaceVolume;
    private float _ambientVolume;
    private bool _muteStateSound;

    public TemporaryData(ConfigData configData) 
    {
        InitData(configData);
    }

    public WeaponData WeaponData { get; private set; }
    public PlayerClassData PlayerClassData { get; private set; }
    public LevelData LevelData { get; private set; }
    public bool MuteStateSound => _muteStateSound;
    public int UpgradePoints => _upgradePoints;
    public int PlayerScore => _playerScore;
    public int CountLevels => _countLevels;
    public string Language => _language;
    public float InterfaceVolume => _interfaceVolume;
    public float AmbientVolume => _ambientVolume;

    public UpgradeState GetUpgradeState(int id)
    {
        if (_upgradeState != null)
        {
            foreach (UpgradeState upgradeState in _upgradeState)
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

    public void SetUpgradeState(UpgradeState[] upgradeStates)
    {
        _upgradeState = upgradeStates;
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
        _ambientVolume = value;
    }

    public void SetInterfaceVolume(float value)
    {
        _interfaceVolume = value;
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
        _levelStates = configData.DefaultLevelState;
        _weaponStates = configData.DefaultWeaponState;
    }
}