using Assets.Source.Game.Scripts;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public int Coins;
        public float AmbientVolume;
        public float SfxVolumeVolume;
        public bool IsMuted;
        //public string DefaultLanguage;
        public List<UpgradeState> UpgradeStates;
        public List<ClassAbilityState> ClassAbilityStates;
        public WeaponState[] DefaultWeaponState;
        public LevelState[] DefaultLevelState;
        public int UpgradePoints;
        public int Score;
        public bool HasSave = false;

        public SavesYG()
        {
        }
    }
}