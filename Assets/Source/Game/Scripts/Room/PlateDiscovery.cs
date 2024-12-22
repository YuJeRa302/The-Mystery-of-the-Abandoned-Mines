using Assets.Source.Game.Scripts;
using System;
using UnityEngine;

public class PlateDiscovery : MonoBehaviour
{
    public event Action PlateEntered;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Player player))
            PlateEntered?.Invoke();
    }
}