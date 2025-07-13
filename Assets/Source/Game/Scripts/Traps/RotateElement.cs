using DG.Tweening;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Traps
{
    public class RotateElement : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed = 3f;
        [SerializeField] private float _xAxisRotation = 0f;
        [SerializeField] private float _yAxisRotation = 360f;
        [SerializeField] private float _zAxisRotation = 0f;

        private void OnEnable()
        {
            transform.DORotate(new Vector3(_xAxisRotation, _yAxisRotation, _zAxisRotation), _rotateSpeed,
                RotateMode.FastBeyond360).SetLoops(-1, LoopType.Yoyo).SetRelative().SetEase(Ease.Linear);
        }
    }
}