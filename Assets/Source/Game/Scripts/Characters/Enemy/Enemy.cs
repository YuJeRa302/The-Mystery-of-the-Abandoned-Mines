using System;
using UnityEngine;

public class Enemy : PoolObject
{
    [SerializeField] private EnemyStateMashineExample _stateMashine;
    [SerializeField] private float _attackDelay;

    private float _health = 20f;
    
    private bool _isDead;
    private float _currentHealth;

    public float AttackDelay => _attackDelay;

    public event Action Died;

    public void Initialize(Player player)
    {
        _currentHealth = _health;
        _stateMashine.InitializeStateMashine(player);
    }

    public void ResetEnemy()
    {
        _isDead = true;
        _currentHealth = _health;
        _stateMashine.ResetState();
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0)
            return;

        if (_currentHealth <= 0)
            return;

        _currentHealth -= damage;
        //GotHit?.Invoke();
        //TakedDamage?.Invoke(damage); ;
        //HealthChanged?.Invoke(_health, _maxHealth);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
            Died?.Invoke();
            ReturnToPool();//test
        }
    }

    protected override void ReturnToPool()
    {
        base.ReturnToPool();
        _isDead = false;
        _currentHealth = _health;
    }
}