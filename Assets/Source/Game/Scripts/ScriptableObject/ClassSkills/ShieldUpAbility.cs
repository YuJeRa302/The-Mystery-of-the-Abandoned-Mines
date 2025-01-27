using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/ShieldUp", order = 51)]
public class ShieldUpAbility : ClassAbilityData
{
    [SerializeField] private PoolParticle _poolParticle;

    public PoolParticle PoolParticle => _poolParticle;
}