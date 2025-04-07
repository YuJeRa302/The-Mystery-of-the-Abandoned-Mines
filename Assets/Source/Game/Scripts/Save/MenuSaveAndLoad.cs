using Assets.Source.Game.Scripts;
using UnityEngine;

public class MenuSaveAndLoad : ISaveAndLoadProgress
{
    private const string DataKeyLocal = "PlayerDataLocalTest";

    private TemporaryData _temporaryData;
    private string _saveData;

    public MenuSaveAndLoad()
    {
        
    }

    public void Initialize(TemporaryData temporaryData)
    {
        _temporaryData = temporaryData;
        _temporaryData.ChengedData += SaveData;
    }

    public void SaveData()
    {
        WeaponState[] weaponStates = new WeaponState[_temporaryData.WeaponStates.Length];

        for (int i = 0; i < _temporaryData.WeaponStates.Length; i++)
        {
            weaponStates[i] = _temporaryData.WeaponStates[i];
        }

        LevelState[] levelStates = new LevelState[_temporaryData.LevelStates.Length];

        for (int i = 0; i < _temporaryData.LevelStates.Length; i++)
        {
            levelStates[i] = _temporaryData.LevelStates[i];
        }

        GameInfo gameInfo = new()
        {
            Coins = _temporaryData.Coins,
            AmbientVolume = _temporaryData.AmbientVolume,
            SfxVolumeVolume = _temporaryData.InterfaceVolume,
            IsMuted = _temporaryData.MuteStateSound,
            DefaultLanguage = _temporaryData.Language,
            UpgradePoints = _temporaryData.UpgradePoints,
            UpgradeStates = new System.Collections.Generic.List<UpgradeState>(_temporaryData.UpgradeStates),
            ClassAbilityStates = new System.Collections.Generic.List<ClassAbilityState>(_temporaryData.ClassAbilityStates),
            DefaultWeaponState = weaponStates,
            DefaultLevelState = levelStates
        };

        _saveData = JsonUtility.ToJson(gameInfo);
        UnityEngine.PlayerPrefs.SetString(DataKeyLocal, _saveData);
        UnityEngine.PlayerPrefs.Save();
    }

    public bool TryGetGameData(out GameInfo gameInfo)
    {
        string data;

        if (UnityEngine.PlayerPrefs.HasKey(DataKeyLocal))
        {
            data = UnityEngine.PlayerPrefs.GetString(DataKeyLocal);
            gameInfo = JsonUtility.FromJson<GameInfo>(data);
            return gameInfo != null;
        }
        else
        {
            data = string.Empty;
            gameInfo = null;
            return false;
        }
    }

    private void ChengeClassAbility(ClassAbilityState classAbilityState)
    {
        
    }
}