namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_PlayerLevelChange
    {
        private int _currentLevel;
        private int _maxExperienceValue;
        private int _currentExperience;

        public M_PlayerLevelChange(int currentLevel, int maxExperienceValue, int currentExperience)
        {
            _currentLevel = currentLevel;
            _maxExperienceValue = maxExperienceValue;
            _currentExperience = currentExperience;
        }

        public int CurrentLevel => _currentLevel;
        public int MaxExperienceValue => _maxExperienceValue;
        public int CurrentExperience => _currentExperience;
    }
}