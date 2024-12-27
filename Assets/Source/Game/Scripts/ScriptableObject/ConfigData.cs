using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Config", menuName = "Create Config", order = 51)]
    public class ConfigData : ScriptableObject
    {
        [SerializeField] private float _ambientVolume;
        [SerializeField] private float _sfxVolumeVolume;
        [SerializeField] private bool _isMuted;
        [SerializeField] private string _defaultLanguage;
        [Space(20)]
        [SerializeField] private WeaponState[] _defaultWeaponState;
        [Space(10)]
        [SerializeField] private LevelState[] _defaultLevelState;
        [Space(20)]
        [SerializeField] private int _upgradePoints;

        public float AmbientVolume => _ambientVolume;
        public float SfxVolumeVolume => _sfxVolumeVolume;
        public bool IsMuted => _isMuted;
        public string DefaultLanguage => _defaultLanguage;
        public WeaponState[] DefaultWeaponState => _defaultWeaponState;
        public LevelState[] DefaultLevelState => _defaultLevelState;
        public int UpgradePoints => _upgradePoints;
    }
}