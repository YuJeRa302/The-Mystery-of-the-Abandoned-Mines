using DG.Tweening;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class BeholderSpecialAttackState : BossSpecialAttackState
    {
        private readonly float _rotationY = -90f;
        private readonly float _duration = 1.5f;

        private float _maxTimeCastSpel = 6f;
        private float _currentTimeCastSpel = 0;
        private Transform _transformEnemy;
        private Sequence _sequence;

        public BeholderSpecialAttackState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
        {
            _transformEnemy = Enemy.transform;
        }

        public override void EnterState()
        {
            base.EnterState();
            _currentTimeCastSpel = 0f;
            Beholder beholder = Enemy as Beholder;
            beholder.DragonFlame.Initialize(beholder.SpecialAttackDamage);
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
                SetTransitStatus(true);
        }
    }
}