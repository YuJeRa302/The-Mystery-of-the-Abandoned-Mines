using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.Models
{
    public struct M_AbilityReseted
    {
        private PlayerClassData _playerClassData;

        public M_AbilityReseted(PlayerClassData playerClassData)
        {
            _playerClassData = playerClassData;
        }

        public PlayerClassData PlayerClassData => _playerClassData;
    }
}