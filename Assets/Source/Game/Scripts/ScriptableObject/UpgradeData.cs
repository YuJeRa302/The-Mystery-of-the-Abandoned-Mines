using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "Create Stats Upgrade", order = 51)]
    public class UpgradeData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private TypeParameter _typeParameter;
        [SerializeField] private List<UpgradeParameter> _upgradeParameters;
        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _description;

        public int Id => _id;
        public string Name => _name;
        public string Description => _description;
        public List<UpgradeParameter> UpgradeParameters => _upgradeParameters;
        public Sprite Icon => _icon;
        public TypeParameter TypeParameter => _typeParameter;

        public UpgradeParameter GetUpgradeParameterByCurrentLevel(int currentLevel) 
        {
            foreach (UpgradeParameter upgradeParameter in _upgradeParameters)
            {
                if (upgradeParameter.CurrentLevel == currentLevel)
                    return upgradeParameter;
            }

            return null;
        }
    }
}