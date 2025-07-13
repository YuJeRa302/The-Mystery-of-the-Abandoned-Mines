using Assets.Source.Game.Scripts.Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New SummonData", menuName = "Create Summon", order = 51)]
    public class SummonData : ScriptableObject
    {
        [SerializeField] private List<int> _healthValues;
        [SerializeField] private List<int> _damageValues;
        [SerializeField] private Summon _prefabSummon;
        [SerializeField] private float _lifeTime;

        public Summon Summon => _prefabSummon;
    }
}