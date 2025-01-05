using UnityEngine;

public class WeponPrefab : MonoBehaviour
{
    [SerializeField] private ParticleSystem _kickEffect;

    public ParticleSystem KickEffect => _kickEffect;
}