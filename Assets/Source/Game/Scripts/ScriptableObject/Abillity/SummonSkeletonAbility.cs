using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New Summon Ability", menuName = "Create Summon Ability", order = 51)]
public class SummonSkeletonAbility : AbilityAttributeData
{
    [SerializeField] private SummonData _summonData;
    [SerializeField] private int _defaultLevel = 0;
    [SerializeField] private int _id;

    public Summon SummonPrefab => _summonData.Summon;
}