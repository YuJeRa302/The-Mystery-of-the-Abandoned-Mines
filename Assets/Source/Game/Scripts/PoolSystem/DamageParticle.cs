using Assets.Source.Game.Scripts;
using UnityEngine;

public class DamageParticle : PoolParticle
{
    private float _damage;

    public void Initialaze(float damage)
    {
        _damage = damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(_damage);
        }
    }
}