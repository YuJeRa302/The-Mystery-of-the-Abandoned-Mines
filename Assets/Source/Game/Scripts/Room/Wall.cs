using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    public void UpdateMaterial(Color color) 
    {
        _meshRenderer.material.color = color;
    }
}