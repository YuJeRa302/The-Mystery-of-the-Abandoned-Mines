using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SummonData", menuName = "Create Summon", order = 51)]
public class SummonData : ScriptableObject
{
    [SerializeField] private List<int> _healthValues;
    [SerializeField] private List<int> _damageValues;
    [SerializeField] private Summon _prefabSummon;
    [SerializeField] private float _lifeTime;

    public List<int> HealthValues => _healthValues;
    public List<int> DamageValues => _damageValues;
    public Summon Summon => _prefabSummon;
    public float LifeTime => _lifeTime;
}