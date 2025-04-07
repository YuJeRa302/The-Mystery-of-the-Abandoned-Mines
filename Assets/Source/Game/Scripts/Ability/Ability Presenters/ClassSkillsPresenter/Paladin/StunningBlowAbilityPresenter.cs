using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class StunningBlowAbilityPresenter : AbilityPresenter
{
    private Coroutine _coroutine;
    private Transform _effectConteiner;
    private Pool _pool;
    private PoolParticle _poolParticle;
    private bool _isAbilityUse;

    public StunningBlowAbilityPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        PoolParticle abilityEffect) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _pool = _player.Pool;
        _poolParticle = abilityEffect;
        _effectConteiner = _player.PlayerAbilityContainer;
        AddListener();
    }

    protected override void AddListener()
    {
        base.AddListener();
        (_abilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
    }

    protected override void RemoveListener()
    {
        base.RemoveListener();
        (_abilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);

        _isAbilityUse = false;
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        _ability.Use();
        (_abilityView as ClassSkillButtonView).SetInerectableButton(false);
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        _isAbilityUse = true;
        CastBlow();
    }

    protected override void OnGameResumed()
    {
        //if (_isAbilityUse)
            base.OnGameResumed();
    }

    private void CastBlow()
    {
        CreateParticle();
        ApplayDamage();
    }

    private void CreateParticle()
    {
        PoolParticle particle;

        if (_pool.TryPoolObject(_poolParticle.gameObject, out PoolObject pollParticle))
        {
            particle = pollParticle as PoolParticle;
            particle.transform.position = _effectConteiner.position;
            particle.gameObject.SetActive(true);
        }
        else
        {
            particle = GameObject.Instantiate(_poolParticle, _effectConteiner);
            _pool.InstantiatePoolObject(particle, _poolParticle.name);
        }
    }

    private void ApplayDamage()
    {
        if (TryFindEnemy(out List<Enemy> findedEnemies))
        {
            foreach (var enemy in findedEnemies)
            {
                enemy.TakeDamageTest(_ability.DamageParametr);
            }
        }
    }

    private bool TryFindEnemy(out List<Enemy> findedEnemies)
    {
        Collider[] coliderEnemy = Physics.OverlapSphere(_player.transform.position, 4f);
        List<Enemy> enemies = new ();

        foreach (Collider collider in coliderEnemy)
        {
            if (collider.TryGetComponent(out Enemy enemy))
                enemies.Add(enemy);
        }

        findedEnemies = enemies;
        return findedEnemies != null;
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (_abilityView as ClassSkillButtonView).SetInerectableButton(true);
    }
}