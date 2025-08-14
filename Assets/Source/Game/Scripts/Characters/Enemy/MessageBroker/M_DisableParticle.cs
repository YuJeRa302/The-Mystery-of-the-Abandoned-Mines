using Assets.Source.Game.Scripts.PoolSystem;

public struct M_DisableParticle
{
    private PoolParticle _poolParticle;

    public M_DisableParticle(PoolParticle poolParticle)
    {
        _poolParticle = poolParticle;
    }

    public PoolParticle PoolParticle => _poolParticle;
}