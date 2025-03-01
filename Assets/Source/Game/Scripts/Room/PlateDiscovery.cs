using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class PlateDiscovery : MonoBehaviour
{
    [SerializeField] private ParticleSystem _effects;
    public event Action PlateEntered;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Player player))
        {
            PlateEntered?.Invoke();
            _effects.gameObject.SetActive(true);
            _effects.Play();
        }
    }
}