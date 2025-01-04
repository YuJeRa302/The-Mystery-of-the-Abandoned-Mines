using Assets.Source.Game.Scripts;
using DG.Tweening;
using UnityEngine;

public class BeholderSpecialAttackState : BossSpecialAttackState
{
    private float _maxTimeCastSpel = 6f;
    private float _currentTimeCastSpel = 0;
    private Transform _transformEnemy;
    DG.Tweening.Sequence _sequence = DOTween.Sequence();

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
        beholder.DragonFlame.gameObject.SetActive(true);
        _sequence.Append(_transformEnemy.DORotate(new Vector3(0, 360f, 0), _maxTimeCastSpel, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear));
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