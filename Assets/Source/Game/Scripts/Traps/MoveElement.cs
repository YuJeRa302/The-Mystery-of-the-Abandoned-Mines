using DG.Tweening;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Traps
{
    public class MoveElement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _xAxisDirection = 0f;
        [SerializeField] private float _yAxisDirection = 0f;
        [SerializeField] private float _zAxisDirection = 2f;

        private void OnEnable()
        {
            transform.DOMove(new Vector3(_xAxisDirection, _yAxisDirection, _zAxisDirection),
                _moveSpeed, false).
                SetLoops(-1, LoopType.Yoyo).SetRelative().SetEase(Ease.Linear);
        }
    }
}