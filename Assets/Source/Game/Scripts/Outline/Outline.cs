using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Outlines
{
    [DisallowMultipleComponent]
    public class Outline : MonoBehaviour
    {
        private static readonly int ZTest = Shader.PropertyToID("_ZTest");
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");

        private readonly int _singleVertices = 1;

        [SerializeField]
        private OutlineMode _outlineMode;
        [SerializeField]
        private Color _outlineColor = Color.white;
        [SerializeField, Range(0f, 10f)]
        private float _outlineWidth = 2f;
        [Header("For Large Objects")]
        [SerializeField]
        private bool _precomputeOutline;
        [SerializeField, HideInInspector]
        private List<Mesh> _bakeKeys = new();
        [SerializeField, HideInInspector]
        private List<ListVector3> _bakeValues = new();

        private Renderer[] _renderers;
        private Material _outlineMaskMaterial;
        private Material _outlineFillMaterial;
        private HashSet<Mesh> _registeredMeshes = new();
        private bool _needsUpdate;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
            _outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));
            _outlineMaskMaterial.name = "OutlineMask (Instance)";
            _outlineFillMaterial.name = "OutlineFill (Instance)";
            LoadSmoothNormals();
            _needsUpdate = true;
        }

        private void OnEnable()
        {
            foreach (var renderer in _renderers)
            {
                var materials = renderer.sharedMaterials.ToList();
                materials.Add(_outlineMaskMaterial);
                materials.Add(_outlineFillMaterial);
                renderer.materials = materials.ToArray();
            }
        }

        private void OnValidate()
        {
            _needsUpdate = true;

            if (!_precomputeOutline && _bakeKeys.Count != 0 || _bakeKeys.Count != _bakeValues.Count)
            {
                _bakeKeys.Clear();
                _bakeValues.Clear();
            }

            if (_precomputeOutline && _bakeKeys.Count == 0)
                Bake();
        }

        private void Update()
        {
            if (_needsUpdate)
            {
                _needsUpdate = false;
                UpdateMaterialProperties();
            }
        }

        private void OnDisable()
        {
            foreach (var renderer in _renderers)
            {
                var materials = renderer.sharedMaterials.ToList();
                materials.Remove(_outlineMaskMaterial);
                materials.Remove(_outlineFillMaterial);
                renderer.materials = materials.ToArray();
            }
        }

        private void OnDestroy()
        {
            Destroy(_outlineMaskMaterial);
            Destroy(_outlineFillMaterial);
        }

        private void Bake()
        {
            var bakedMeshes = new HashSet<Mesh>();

            foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
            {
                if (!bakedMeshes.Add(meshFilter.sharedMesh))
                    continue;

                var smoothNormals = SmoothNormals(meshFilter.sharedMesh);
                _bakeKeys.Add(meshFilter.sharedMesh);
                _bakeValues.Add(new ListVector3(smoothNormals));
            }
        }

        private void LoadSmoothNormals()
        {
            foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
            {
                if (!_registeredMeshes.Add(meshFilter.sharedMesh))
                    continue;

                var index = _bakeKeys.IndexOf(meshFilter.sharedMesh);
                var smoothNormals = index >= 0 ? _bakeValues[index].Data : SmoothNormals(meshFilter.sharedMesh);
                meshFilter.sharedMesh.SetUVs(3, smoothNormals);
                var renderer = meshFilter.GetComponent<Renderer>();

                if (renderer != null)
                    CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }

            foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (!_registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                    continue;

                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
                CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
            }
        }

        private List<Vector3> SmoothNormals(Mesh mesh)
        {
            var groups = mesh.vertices.Select((vertex, index) =>
            new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);
            var smoothNormals = new List<Vector3>(mesh.normals);

            foreach (var group in groups)
            {
                if (group.Count() == _singleVertices)
                    continue;

                var smoothNormal = Vector3.zero;

                foreach (var pair in group)
                {
                    smoothNormal += smoothNormals[pair.Value];
                }

                smoothNormal.Normalize();

                foreach (var pair in group)
                {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }

            return smoothNormals;
        }

        private void CombineSubmeshes(Mesh mesh, Material[] materials)
        {
            if (mesh.subMeshCount == _singleVertices)
                return;

            if (mesh.subMeshCount > materials.Length)
                return;

            mesh.subMeshCount++;
            mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
        }

        private void UpdateMaterialProperties()
        {
            _outlineFillMaterial.SetColor("_OutlineColor", _outlineColor);

            switch (_outlineMode)
            {
                case OutlineMode.OutlineAll:
                    _outlineMaskMaterial.SetFloat(ZTest,
                        (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat(ZTest,
                        (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat(OutlineWidth,
                        _outlineWidth);
                    break;

                case OutlineMode.OutlineVisible:
                    _outlineMaskMaterial.SetFloat(ZTest,
                        (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat(ZTest,
                        (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    _outlineFillMaterial.SetFloat(OutlineWidth, _outlineWidth);
                    break;

                case OutlineMode.OutlineHidden:
                    _outlineMaskMaterial.SetFloat(ZTest,
                        (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat(ZTest, 
                       (float)UnityEngine.Rendering.CompareFunction.Greater);
                    _outlineFillMaterial.SetFloat(OutlineWidth,
                        _outlineWidth);
                    break;

                case OutlineMode.OutlineAndSilhouette:
                    _outlineMaskMaterial.SetFloat(ZTest,
                        (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    _outlineFillMaterial.SetFloat(ZTest,
                        (float)UnityEngine.Rendering.CompareFunction.Always);
                    _outlineFillMaterial.SetFloat(OutlineWidth,
                        _outlineWidth);
                    break;

                case OutlineMode.SilhouetteOnly:
                    _outlineMaskMaterial.SetFloat(ZTest, 
                       (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    _outlineFillMaterial.SetFloat(ZTest, 
                       (float)UnityEngine.Rendering.CompareFunction.Greater);
                    _outlineFillMaterial.SetFloat(OutlineWidth, 0f);
                    break;
            }
        }
    }
}