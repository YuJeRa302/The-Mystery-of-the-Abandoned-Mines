using Assets.Source.Game.Scripts.Characters;
using UnityEngine;

public class DragonSpell : MonoBehaviour
{
    [SerializeField] private float _findPlayer = 4f;
    [SerializeField] private ParticleSystem _effect;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _findPlayer);
    }

    public bool TryFindPlayer(out Player player)
    {
        _effect.Play();
        Collider[] coliders = Physics.OverlapSphere(transform.position, _findPlayer);

        foreach (Collider collider in coliders)
        {
            if (collider.TryGetComponent(out player))
                return true;
        }

        player = null;
        return false;
    }
}