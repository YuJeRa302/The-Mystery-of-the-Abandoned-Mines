using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_ClassAbilityViewCreat
    {
        private ClassAbilityData _classAbilityData;
        private ClassSkillButtonView _classSkillButtonView;
        private int _currentLvl;

        public M_ClassAbilityViewCreat(
            ClassAbilityData classAbilityData,
            ClassSkillButtonView classSkillButtonView,
            int currentLvl)
        {
            _classAbilityData = classAbilityData;
            _classSkillButtonView = classSkillButtonView;
            _currentLvl = currentLvl;
        }

        public ClassAbilityData ClassAbilityData => _classAbilityData;
        public ClassSkillButtonView ClassSkillButtonView => _classSkillButtonView;
        public int CurrentLvl => _currentLvl;
    }
}