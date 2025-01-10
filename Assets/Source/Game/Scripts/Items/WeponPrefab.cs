using UnityEngine;

public class WeponPrefab : MonoBehaviour
{
    [SerializeField] private PoolParticle _kickEffect;

    public PoolParticle KickEffect => _kickEffect;
}