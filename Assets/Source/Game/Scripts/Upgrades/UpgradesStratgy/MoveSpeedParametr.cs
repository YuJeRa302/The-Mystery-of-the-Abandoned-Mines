using Assets.Source.Game.Scripts.Services;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class MoveSpeedParametr : IUpgradeStats, IRevertStats
    {
        private readonly float _moveSpeedAmplifier = 2f;

        private float _moveSpeed;
        private float _maxMoveSpeed;
        private float _ownSpeed;

        public MoveSpeedParametr(float baseValue)
        {
            _moveSpeed = baseValue;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
        }

        public float CurrentMoveSpeed => _moveSpeed;
        public float MaxMoveSpeed => _maxMoveSpeed;

        public void Apply(float value)
        {
            if (value == 0)
                ChangeMoveSpeed(value);
            else
                IncreaseMoveSpeed(value);
        }

        public void Revent(float value)
        {
            if (value == 0)
                ChangeMoveSpeed(_ownSpeed);
            else
                DecreaseMoveSpeed(value);
        }

        private void IncreaseMoveSpeed(float value)
        {
            _moveSpeed += value;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
        }

        private void DecreaseMoveSpeed(float value)
        {
            _moveSpeed -= value;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
        }

        private void ChangeMoveSpeed(float value)
        {
            _ownSpeed = _moveSpeed;
            _moveSpeed = value;
            _maxMoveSpeed = _moveSpeed * _moveSpeedAmplifier;
        }
    }
}