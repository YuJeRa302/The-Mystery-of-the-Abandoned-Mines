using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public struct M_EnemyFind
    {
        private Transform _transform;

        public M_EnemyFind(Transform transform)
        {
            _transform = transform;
        }

        public Transform Transform => _transform;
    }
}