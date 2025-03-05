using Assets.Source.Game.Scripts;
using UnityEngine;

public class PlayerBullet : Bullet
{
    private Enemy _target;

    private void FixedUpdate()
    {
        Vector3.Magnitude(transform.position - _target.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.collider.TryGetComponent(out Enemy enemy))
        //{
        //    enemy.TakeDamage(_damage);
        //    ReturObjectPool();
        //}

        //if (collision.collider.TryGetComponent(out Wall wall))
        //    ReturObjectPool();
    }
}