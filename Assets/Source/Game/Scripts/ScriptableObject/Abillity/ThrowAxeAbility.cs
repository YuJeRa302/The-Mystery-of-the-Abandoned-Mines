using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New AxeThrowAbility", menuName = "Create AxeThrow Ability", order = 51)]
public class ThrowAxeAbility : AbilityAttributeData
{
    [SerializeField] private AxemMssile _axemMssile;

    public AxemMssile AxemMssile => _axemMssile;
}