using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.AbilityScripts
{
    public class AbilityEntitiesHolder
    {
        private readonly PlayerAbilityCaster _playerAbilityCaster;

        public AbilityEntitiesHolder(PlayerAbilityCaster playerAbilityCaster)
        {
            _playerAbilityCaster = playerAbilityCaster;
        }

        public Player Player => _playerAbilityCaster.Player;
        public Ability Ability => _playerAbilityCaster.Ability;
        public AbilityView AbilityView => _playerAbilityCaster.AbilityView;
        public AttributeData AttributeData => _playerAbilityCaster.AttributeData;
        public ParticleSystem ParticleSystem => _playerAbilityCaster.ParticleSystem;
        public IAbilityStrategy IAbilityStrategy => _playerAbilityCaster.IAbilityStrategy;
    }
}