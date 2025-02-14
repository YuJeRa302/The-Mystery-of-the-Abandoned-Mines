using Assets.Source.Game.Scripts;
using UnityEngine;

public class DamageParticle : PoolParticle
{
    private DamageParametr _damageParametr;

    public void Initialaze(DamageParametr damageParametr)
    {
        _damageParametr = damageParametr;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamageTest(_damageParametr);
        }
    }
}