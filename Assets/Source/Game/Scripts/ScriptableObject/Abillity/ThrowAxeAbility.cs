using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New AxeThrowAbility", menuName = "Create AxeThrow Ability", order = 51)]
public class ThrowAxeAbility : AbilityAttributeData
{
    [SerializeField] private AxemMssile _axemMssile;
    [SerializeField] private Spell _spell;

    public Spell Spell => _spell;
    public AxemMssile AxemMssile => _axemMssile;
}