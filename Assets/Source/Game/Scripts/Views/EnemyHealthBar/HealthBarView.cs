using Assets.Source.Game.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Slider _bar;
    [SerializeField] private Transform _damagePopupText;
    [SerializeField] private DamagePopup _damagePopupPrefab;
    [SerializeField] private Pool _pool;

    private Enemy _enemy;

    private void OnEnable()
    {
        if (_enemy != null)
        {
            _enemy.DamageTaked += OnTakeDamage;
            _enemy.HealthChanged += OnChangeHealthValue;
        }
    }

    private void OnDisable()
    {
        _enemy.DamageTaked -= OnTakeDamage;
        _enemy.HealthChanged -= OnChangeHealthValue;
    }

    public void Initialize(Enemy enemy)
    {
        _enemy = enemy;
        _enemy.DamageTaked += OnTakeDamage;
        _enemy.HealthChanged += OnChangeHealthValue;
    }

    private void OnChangeHealthValue()
    {
        float amount = _enemy.CurrentHealth / (float)_enemy.MaxHealth;
        _bar.value = amount;
    }

    private void OnTakeDamage(float damage)
    {
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