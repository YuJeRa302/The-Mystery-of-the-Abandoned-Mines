using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/JerkFront", order = 51)]
public class JerkFrontAbilityData : ClassAbilityData
{
    [SerializeField] private PoolParticle _poolParticle;

    public PoolParticle PoolParticle => _poolParticle;
}