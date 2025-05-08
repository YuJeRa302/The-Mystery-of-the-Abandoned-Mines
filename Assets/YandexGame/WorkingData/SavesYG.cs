
using Assets.Source.Game.Scripts;
using System.Collections.Generic;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        // Тестовые сохранения для демо сцены
        // Можно удалить этот код, но тогда удалите и демо (папка Example)
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

        public SavesYG()
        {
        }
    }
}