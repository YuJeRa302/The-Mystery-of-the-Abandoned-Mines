using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/StunningBlow", order = 51)]
public class StunningBlowClassAbilityData : ClassAbilityData
{
    [SerializeField] private PoolParticle _poolParticle;

    public PoolParticle PoolParticle => _poolParticle;
}
