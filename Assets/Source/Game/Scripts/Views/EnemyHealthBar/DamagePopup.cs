using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopup : PoolObject
{
    private readonly int _loopsCpunt = 2;
    private readonly float _durationScale = 0.2f;
    private readonly float _scaleValue = 1.2f;
    private readonly float _minOffsetX = 1f;
    private readonly float _maxOffsetX = 5f;
    private readonly float _durationMoveZ = 1f;
    private readonly float _offsetMoveZ = 2f;

    [SerializeField] private TextMeshPro _text;

    public void Initialize(string damageVale)
    {
        _text.text = damageVale;
        transform.position += new Vector3(Random.Range(_minOffsetX, _maxOffsetX), 0, 0);
        transform.DOScale(_scaleValue, _durationScale).SetLoops(_loopsCpunt, LoopType.Yoyo);
        transform.DOMoveZ(transform.position.z + _offsetMoveZ, _durationMoveZ).OnComplete(() => ReturnToPool());
    }
}