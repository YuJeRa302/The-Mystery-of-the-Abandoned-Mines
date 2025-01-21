using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/Rage", order = 51)]
public class RageClassAbilityData : ClassAbilityData
{
    [SerializeField] private PoolParticle _rageEffect;

    public PoolParticle RageEffect => _rageEffect;
}