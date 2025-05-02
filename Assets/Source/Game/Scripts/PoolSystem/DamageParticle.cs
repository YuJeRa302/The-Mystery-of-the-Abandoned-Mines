using Assets.Source.Game.Scripts;
using UnityEngine;

public class DamageParticle : PoolParticle
{
    private DamageSource _damageParametr;

    public void Initialaze(DamageSource damageParametr)
    {
        _damageParametr = damageParametr;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(_damageParametr);
        }
    }
}