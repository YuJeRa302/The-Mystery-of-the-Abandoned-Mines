using Assets.Source.Game.Scripts;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SaveAndLoader : ISaveAndLoadProgress
{
    private const string DataKeyLocal = "PlayerDataLocalTest";

    private TemporaryData _temporaryData;
    private string _saveData;

    public SaveAndLoader()
    {
    }

    public void Initialize(TemporaryData temporaryData)
    {
        _temporaryData = temporaryData;
        _temporaryData.ChengedData += async () => await SaveData();
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

    public async UniTask SaveData()
    {
        var serializedData = await UniTask.Run(() =>
        {
            var weaponStates = new WeaponState[_temporaryData.WeaponStates.Length];
            Array.Copy(_temporaryData.WeaponStates, weaponStates, weaponStates.Length);

            var levelStates = new LevelState[_temporaryData.LevelStates.Length];
            Array.Copy(_temporaryData.LevelStates, levelStates, levelStates.Length);

            var gameInfo = new GameInfo
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

            return JsonUtility.ToJson(gameInfo);
        });

        UnityEngine.PlayerPrefs.SetString(DataKeyLocal, serializedData);
        UnityEngine.PlayerPrefs.Save();
    }

    public async UniTask<(bool Success, GameInfo Data)> TryGetGameData()
    {
        if (!UnityEngine.PlayerPrefs.HasKey(DataKeyLocal))
            return (false, null);

        string json = UnityEngine.PlayerPrefs.GetString(DataKeyLocal);
        GameInfo loadedData = await UniTask.Run(() => JsonUtility.FromJson<GameInfo>(json));

        return (loadedData != null, loadedData);
    }
}