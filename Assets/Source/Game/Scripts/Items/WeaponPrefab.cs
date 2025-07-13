using Assets.Source.Game.Scripts.PoolSystem;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Items
{
    public class WeaponPrefab : MonoBehaviour
    {
        [SerializeField] private PoolParticle _kickEffect;

        public PoolParticle KickEffect => _kickEffect;
    }
}