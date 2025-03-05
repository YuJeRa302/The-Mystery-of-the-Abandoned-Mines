using Assets.Source.Game.Scripts;
using UnityEngine;

public class ThrowAxeAbilityPresenter : AbilityPresenter
{
    private Transform _throwPoint;
    private Pool _pool;
    private AxemMssile _axemMssile;
    private bool _isAbilityUse;

    public ThrowAxeAbilityPresenter(Ability ability,
        AbilityView abilityView,
        Player player,
        IGameLoopService gameLoopService,
        ICoroutineRunner coroutineRunner, AxemMssile axemMssile) : base(ability, abilityView, player, gameLoopService, coroutineRunner)
    {
        _throwPoint = _player.ThrowAbilityPoint;
        _pool = _player.Pool;
        _axemMssile = axemMssile;
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
        Spawn();
    }

    private void Spawn()
    {
        AxemMssile axemMssile = null;

        if (TryFindSummon(_axemMssile.gameObject, out AxemMssile poolAxe))
        {
            axemMssile = poolAxe;
            axemMssile.transform.position = _throwPoint.position;
            axemMssile.gameObject.SetActive(true);
            axemMssile.ThrowNow();
        }
        else
        {
            axemMssile = GameObject.Instantiate(_axemMssile, _throwPoint.position, UnityEngine.Quaternion.identity);

            _pool.InstantiatePoolObject(axemMssile, _axemMssile.name);
            axemMssile.Initialaze(_player, _player.PlayerAttacker.DamageParametr, _player.PlayerMovment.MoveSpeed);
        }

        axemMssile.GetComponent<Rigidbody>().AddForce(_throwPoint.forward * _ability.CurrentDuration, ForceMode.Impulse);
    }

    private bool TryFindSummon(GameObject type, out AxemMssile poolObj)
    {
        poolObj = null;

        if (_pool.TryPoolObject(type, out PoolObject axePool))
        {
            poolObj = axePool as AxemMssile;
        }

        return poolObj != null;
    }

    protected override void OnCooldownValueReseted(float value)
    {
        base.OnCooldownValueReseted(value);
        (_abilityView as ClassSkillButtonView).SetInerectableButton(true);
    }
}