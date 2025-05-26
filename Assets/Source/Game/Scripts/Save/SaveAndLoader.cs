using Assets.Source.Game.Scripts;
using System;
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

    public bool TryGetGameData(out SavesYG gameInfo)
    {
        gameInfo = YandexGame.savesData;

        return YandexGame.savesData.isFirstSession;
    }

    public void SaveData()
    {
        var weaponStates = new WeaponState[_temporaryData.WeaponStates.Length];
        Array.Copy(_temporaryData.WeaponStates, weaponStates, weaponStates.Length);

        var levelStates = new LevelState[_temporaryData.LevelStates.Length];
        Array.Copy(_temporaryData.LevelStates, levelStates, levelStates.Length);
        
        YandexGame.savesData = new SavesYG
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

        YandexGame.SaveProgress();
    }
}