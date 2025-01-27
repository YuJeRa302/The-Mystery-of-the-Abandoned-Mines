using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/DarkPact", order = 51)]
public class DarkPactAbilityData : ClassAbilityData
{
    [SerializeField] private PoolParticle _poolParticle;

    public PoolParticle PoolParticle => _poolParticle;
}