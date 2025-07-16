using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Outlines
{
    [System.Serializable]
    public class ListVector3
    {
        [SerializeField] private List<Vector3> _data;

        public ListVector3(List<Vector3> vector3)
        {
            _data = vector3;
        }

        public List<Vector3> Data => _data;
    }
}