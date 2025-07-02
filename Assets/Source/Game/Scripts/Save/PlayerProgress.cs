using System;

[Serializable]
public class PlayerProgress
{
    public int Coins;
    public int UpgradePoints;
    public int Score;
    public string Language;
    public float AmbientVolume;
    public float SfxVolume;
    public bool IsMuted;
    public bool IsGamePause = false;
    public int CurrentPlayerClassId;
    public LevelService LevelService;
    public UpgradeService UpgradeService;
    public WeaponService WeaponService;
    public ClassAbilityService ClassAbilityService;

    public PlayerProgress() 
    {
        UpgradeService = new ();
        WeaponService = new ();
        LevelService = new ();
        ClassAbilityService = new ();
    }
}
