using Assets.Source.Game.Scripts;
using DG.Tweening;
using UnityEngine;

public class BeholderSpecialAttackState : BossSpecialAttackState
{
    private readonly float _rotationY = -90f;
    private readonly float _duration = 1.5f;

    private float _maxTimeCastSpel = 6f;
    private float _currentTimeCastSpel = 0;
    private Transform _transformEnemy;
    private DG.Tweening.Sequence _sequence;

    public BeholderSpecialAttackState(StateMashine stateMashine, Player player, Enemy enemy) : base(stateMashine, player, enemy)
    {
        Target = player;
        Enemy = enemy;
        _transformEnemy = Enemy.transform;
        AnimationController = Enemy.AnimationStateController;
    }

    public override void EnterState()
    {
        base.EnterState();
        _currentTimeCastSpel = 0f;
        Beholder beholder = Enemy as Beholder;
        beholder.DragonFlame.Initialize(beholder.SpecilAttackDamage);
        beholder.DragonFlame.gameObject.SetActive(true);
        
        _sequence = DOTween.Sequence();
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, _rotationY, 0), _duration).SetRelative());
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, -_rotationY, 0), _duration).SetRelative());
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, -_rotationY, 0), _duration).SetRelative());
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, _rotationY, 0), _duration).SetRelative());

        SpetiallAttackEvent();
    }

    public override void ExitState()
    {
        base.ExitState();
        DOTween.Kill(_sequence);
        _sequence.Kill();
        
        Beholder beholder = Enemy as Beholder;
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
            CanTransit = true;
    }
}