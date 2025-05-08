using Assets.Source.Game.Scripts;
using DG.Tweening;
using UnityEngine;

public class BeholderSpecialAttackState : BossSpecialAttackState
{
    private float _maxTimeCastSpel = 6f;
    private float _currentTimeCastSpel = 0;
    private Transform _transformEnemy;
    DG.Tweening.Sequence _sequence;

    public BeholderSpecialAttackState(StateMashine stateMashine, Player player, Enemy enemy) : base(stateMashine, player, enemy)
    {
        _target = player;
        _enemy = enemy;
        _transformEnemy = _enemy.transform;
        _animationController = _enemy.AnimationStateController;
    }

    public override void EnterState()
    {
        base.EnterState();
        _currentTimeCastSpel = 0f;
        Beholder beholder = _enemy as Beholder;
        beholder.DragonFlame.Initialize(beholder.SpecilAttackDamage);
        beholder.DragonFlame.gameObject.SetActive(true);
        
        _sequence = DOTween.Sequence();
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, 90f, 0), 1.5f).SetRelative());
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, -90f, 0), 1.5f).SetRelative());
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, -90f, 0), 1.5f).SetRelative());
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, 90f, 0), 1.5f).SetRelative());

        SpetiallAttackEvent();
    }

    public override void ExitState()
    {
        base.ExitState();
        DOTween.Kill(_sequence);
        _sequence.Kill();
        
        Beholder beholder = _enemy as Beholder;
        beholder.DragonFlame.gameObject.SetActive(false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        CastSpel();
    }

    private void CastSpel()
    {
        _currentTimeCastSpel += Time.deltaTime;

        if (_currentTimeCastSpel >= _maxTimeCastSpel)
            _canTransit = true;
    }
}