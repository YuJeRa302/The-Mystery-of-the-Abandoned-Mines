using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class PlateDiscovery : MonoBehaviour
{
    [SerializeField] private ParticleSystem _effects;

    private bool _isUsed = false;

    public event Action<Player> PlateEntered;

    private void OnCollisionEnter(Collision collision)
    {
        if (_isUsed == false)
        {
            if (collision.collider.TryGetComponent(out Player player))
            {
                PlateEntered?.Invoke(player);
                _effects.gameObject.SetActive(true);
                _effects.Play();
                _isUsed = true;
            }
        }
    }
}