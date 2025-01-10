using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PoolParticle : PoolObject
{
    private ParticleSystem _particleSystem;

    private void OnEnable()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public override void InitializeObject(string name)
    {
        base.InitializeObject(name);
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleSystemStopped()
    {
        ReturnToPool();
    }
}