using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopup : PoolObject
{
    [SerializeField] private TextMeshPro _text;

    public void Initialize(string damageVale)
    {
        _text.text = damageVale;
        transform.position += new Vector3(Random.Range(1f, 5f), 0, 0);
        transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
        transform.DOMoveZ(transform.position.z + 2f, 1f).OnComplete(() => ReturnToPool());
    }
}