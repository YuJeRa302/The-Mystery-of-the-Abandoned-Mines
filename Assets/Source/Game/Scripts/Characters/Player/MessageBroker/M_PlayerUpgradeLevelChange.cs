namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_PlayerUpgradeLevelChange
    {
        private int _currentUpgradeLevel;
        private int _maxExperienceValue;
        private int _currentUpgradeExperience;

        public M_PlayerUpgradeLevelChange(int currentLevel, int maxExperienceValue, int currentExperience)
        {
            _currentUpgradeLevel = currentLevel;
            _maxExperienceValue = maxExperienceValue;
            _currentUpgradeExperience = currentExperience;
        }

        public int CurrentUpgradeLevel => _currentUpgradeLevel;
        public int MaxExperienceValue => _maxExperienceValue;
        public int CurrentUpgradeExperience => _currentUpgradeExperience;
    }
}