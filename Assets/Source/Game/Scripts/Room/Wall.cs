using UnityEngine;

namespace Assets.Source.Game.Scripts.Rooms
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        public void UpdateMaterial(Color color)
        {
            _meshRenderer.material.color = color;
        }
    }
}