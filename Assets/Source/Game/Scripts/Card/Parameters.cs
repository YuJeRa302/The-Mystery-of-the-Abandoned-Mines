using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Card
{
    [System.Serializable]
    public class Parameters
    {
        [SerializeField] private List<CardParameter> _cardParameters;

        public List<CardParameter> CardParameters => _cardParameters;
    }
}