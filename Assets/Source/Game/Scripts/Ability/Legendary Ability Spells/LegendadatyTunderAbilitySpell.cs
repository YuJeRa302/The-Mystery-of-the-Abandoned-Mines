using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class LegendadatyTunderAbilitySpell : LegendaryAbilitySpell
{
    protected ParticleSystem _effect;

    public override void Initialize(ParticleSystem particleSystem, float currentDuration)
    {
        if (_abilityEffects.Count > 0)
        {
            foreach (var abilityEffect in _abilityEffects)
            {
                if (abilityEffect.gameObject != null)
                    Destroy(abilityEffect);
            }
        }

        _effect = particleSystem;
        _spellLifeTime = currentDuration;
        Destroy(gameObject, _spellLifeTime);
    }

    public override bool TryFindEnemys(out List<Enemy> enemies)
    {
        _enemies.Clear();
        enemies = new List<Enemy>();

        foreach (AbilityEnemyFinder abilityEnemyFinder in _abilityEnemyFinders)
        {
            if (abilityEnemyFinder != null)
            {
                if (abilityEnemyFinder.TryFindEnemys(out List<Enemy> findEnemyse))
                {
                    _enemies.AddRange(findEnemyse);
                }
            }
        }

        enemies.AddRange(_enemies);
        
        if (enemies.Count > 0)
        {
            CreateEffect();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateEffect()
    {
        foreach (Enemy enemy in _enemies)
        {
            ParticleSystem abilityEffect = Instantiate(_effect, enemy.transform.position, Quaternion.identity);
            abilityEffect.Play();
            _abilityEffects.Add(abilityEffect);
        }
        
        DestroyEffectsByLifeTime(_spellLifeTime);
    }
}