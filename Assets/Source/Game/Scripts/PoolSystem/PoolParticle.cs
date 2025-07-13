using UnityEngine;

namespace Assets.Source.Game.Scripts.PoolSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PoolParticle : PoolObject
    {
        private void OnParticleSystemStopped()
        {
            ReturnToPool();
        }
    }
}