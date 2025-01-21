using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/Throw Axe", order = 51)]
public class ThrowAxeClassAbility : ClassAbilityData
{
    [SerializeField] private AxemMssile _axemMssile;

    public AxemMssile AxemMssile => _axemMssile;
}