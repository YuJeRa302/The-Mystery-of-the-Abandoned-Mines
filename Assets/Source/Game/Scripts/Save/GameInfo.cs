using Assets.Source.Game.Scripts;
using System.Collections.Generic;

[System.Serializable]
public class GameInfo 
{
    public int Coins;
    public float AmbientVolume;
    public float SfxVolumeVolume;
    public bool IsMuted;
    public string DefaultLanguage;
    public List<UpgradeState> UpgradeStates;
    public List<ClassAbilityState> ClassAbilityStates;
    public WeaponState[] DefaultWeaponState;
    public LevelState[] DefaultLevelState;
    public int UpgradePoints;
}