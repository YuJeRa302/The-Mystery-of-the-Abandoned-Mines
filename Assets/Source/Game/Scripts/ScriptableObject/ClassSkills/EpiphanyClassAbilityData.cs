using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/Epiphany", order = 51)]
public class EpiphanyClassAbilityData : ClassAbilityData
{
    [SerializeField] private DamageParticle _epiphanyParticle;

    public DamageParticle EpiphanyParticle => _epiphanyParticle;
}