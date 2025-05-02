using Assets.Source.Game.Scripts;
using UnityEngine;

public class DragonFlame : MonoBehaviour
{
    [SerializeField] private int _damage;
    private float _damageInterval = 0.5f; // �������� ����� ������� (� ��������)
    private float _lastDamageTime;

    private void OnParticleCollision(GameObject other)
    {
        if (Time.time - _lastDamageTime > _damageInterval)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                Debug.Log("Player");
                player.TakeDamage(_damage);
                _lastDamageTime = Time.time;
            }
        }
    }
}