using Assets.Source.Game.Scripts;
using UnityEngine;

public class SummonAbillityPresenter : AbilityPresenter
{
    private Summon _summonPrefab;
    private Transform _spawnPoint;
    private Pool _pool;
    private bool _isAbilityUse;

    public SummonAbillityPresenter(
        Ability ability,
        AbilityView abilityView,
        Player player,
        GamePauseService gamePauseService,
        GameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner,
        Summon summonPrefab) : base(ability, abilityView, player, gamePauseService, gameLoopService, coroutineRunner)
    {
        _pool = Player.Pool;
        _summonPrefab = summonPrefab;
        _spawnPoint = Player.ShotPoint;
        AddListener();
    }

    protected override void AddListener()
    {
        base.AddListener();
        (AbilityView as ClassSkillButtonView).AbilityUsed += OnButtonSkillClick;
    }

    protected override void RemoveListener()
    {
        base.RemoveListener();
        (AbilityView as ClassSkillButtonView).AbilityUsed -= OnButtonSkillClick;
    }

    protected override void OnAbilityEnded(Ability ability)
    {
        _isAbilityUse = false;
    }

    private void OnButtonSkillClick()
    {
        if (_isAbilityUse)
            return;

        _isAbilityUse = true;
        Ability.Use();
        (AbilityView as ClassSkillButtonView).SetInerectableButton(false);
    }

    protected override void OnAbilityUsed(Ability ability)
    {
        for (int i = 0; i < Ability.Quantily; i++)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        Summon summon = null;

        if (TryFindSummon(_summonPrefab.gameObject, out Summon poolSummon))
        {
            summon = poolSummon;
            summon.transform.position = _spawnPoint.position;
            summon.gameObject.SetActive(true);
            summon.ResetStats();
        }
        else
        {
            summon = GameObject.Instantiate(_summonPrefab, _spawnPoint.position, _spawnPoint.rotation);

            _pool.InstantiatePoolObject(summon, _summonPrefab.name);
            summon.Initialize(Player, Player.DamageSource, Ability.CurrentDuration);
        }
    }

    private bool TryFindSummon(GameObject enemyType, out Summon poolSummon)
    {
        poolSummon = null;

        if (_pool.TryPoolObject(enemyType, out PoolObject enemyPool))
        {
            poolSummon = enemyPool as Summon;
        }

        return poolSummon != null;
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (AbilityView as ClassSkillButtonView).SetInerectableButton(true);
    }
}