using Assets.Source.Game.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Slider _bar;
    [SerializeField] private Transform _damagePopupText;
    [SerializeField] private DamagePopup _damagePopupPrefab;
    [SerializeField] private Pool _pool;

    private Enemy _helth;

    private void OnEnable()
    {
        if (_helth != null)
        {
            _helth.DamageTaked += OnTakeDamage;
            _helth.HealthChenged += OnChangeHealthValue;
        }
    }

    private void OnDisable()
    {
        _helth.DamageTaked -= OnTakeDamage;
        _helth.HealthChenged -= OnChangeHealthValue;
    }

    public void Initialize(Enemy enemy)
    {
        _helth = enemy;
        _helth.DamageTaked += OnTakeDamage;
        _helth.HealthChenged += OnChangeHealthValue;
    }

    private void OnChangeHealthValue()
    {
        float amount = _helth.CurrentHealth / (float)_helth.MaxHealth;
        _bar.value = amount;
    }

    private void OnTakeDamage(float damage)
    {
        //_damagePopupText.alpha = 255f;
        //_damagePopupText.text = damage.ToString();
        //_damagePopupText.DOFade(0f, 1.5f).SetEase(Ease.Linear);
        InstantiateDamagePopup(damage);
    }

    private void InstantiateDamagePopup(float damage)
    {
        DamagePopup popup;

        if (_pool.TryPoolObject(_damagePopupPrefab.gameObject, out PoolObject pollDamage))
        {
            popup = pollDamage as DamagePopup;
            popup.transform.position = _damagePopupText.position;
            popup.gameObject.SetActive(true);
        }
        else
        {
            popup = Instantiate(_damagePopupPrefab, _damagePopupText);
            _pool.InstantiatePoolObject(popup, _damagePopupPrefab.name);
        }

        popup.Initialize(damage.ToString());
    }
}