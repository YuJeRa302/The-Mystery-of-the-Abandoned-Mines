using Assets.Source.Game.Scripts.Items;
using Assets.Source.Game.Scripts.Levels;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Game.Scripts.Upgrades;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Config", menuName = "Create Config", order = 51)]
    public class ConfigData : ScriptableObject
    {
        [SerializeField] private int _upgradePoints;
        [SerializeField] private int _coins;
        [SerializeField] private float _ambientVolume;
        [SerializeField] private float _sfxVolumeVolume;
        [SerializeField] private bool _isMuted;
        [SerializeField] private string _defaultLanguage;
        [Space(20)]
        [SerializeField] private List<UpgradeState> _upgradeStates;
        [Space(20)]
        [SerializeField] private List<ClassAbilityState> _classAbilityStates;
        [Space(10)]
        [SerializeField] private WeaponState[] _defaultWeaponState;
        [Space(10)]
        [SerializeField] private LevelState[] _defaultLevelState;

        public int Coins => _coins;
        public float AmbientVolume => _ambientVolume;
        public float SfxVolumeVolume => _sfxVolumeVolume;
        public bool IsMuted => _isMuted;
        public List<UpgradeState> UpgradeStates => _upgradeStates;
        public List<ClassAbilityState> ClassAbilityStates => _classAbilityStates;
        public WeaponState[] DefaultWeaponState => _defaultWeaponState;
        public LevelState[] DefaultLevelState => _defaultLevelState;
        public int UpgradePoints => _upgradePoints;
    }
}