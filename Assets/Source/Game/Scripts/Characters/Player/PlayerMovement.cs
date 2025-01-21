using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : IDisposable
{
    private float _moveSpeed;

    private Camera _camera;
    private VariableJoystick _variableJoystick;

    private float _maxMoveSpeed;
    private PlayerInput _playerInputSystem;
    private Vector3 _direction;
    private Rigidbody _rigidbody;
    private InputAction _move;
    private Coroutine _movment;
    private ICoroutineRunner _coroutineRunner;
    private bool _isModile = false;
    private bool _canRotate = true;
    private bool _isDecstop = true; //

    public float MaxMoveSpeed => _maxMoveSpeed;
    public float MoveSpeed => _moveSpeed;

    public PlayerMovement(Camera camera, VariableJoystick variableJoystick, Rigidbody rigidbody, float moveSpeed, Player coroutineRunner)
    {
        _moveSpeed = moveSpeed;
        _rigidbody = rigidbody;
        _camera = camera;
        _variableJoystick = variableJoystick;
        _coroutineRunner = coroutineRunner;

        if (_playerInputSystem == null)
        {
            _playerInputSystem = new PlayerInput();
            _playerInputSystem.Enable();
        }

        _move = _playerInputSystem.Player.Move;
        _maxMoveSpeed = _moveSpeed * 2f;

        _movment = _coroutineRunner.StartCoroutine(DekstopMove());
    }

    public void Dispose()
    {
        if (_playerInputSystem != null)
            _playerInputSystem.Disable();

        if (_movment != null)
            _coroutineRunner.StopCoroutine(_movment);

        GC.SuppressFinalize(this);
    }

    public void ChangeMoveSpeed(float value)
    {
        _moveSpeed += value;
        Debug.Log(_moveSpeed);
        _maxMoveSpeed = _moveSpeed * 2f;

    }

    public void ChengeRotate()
    {
        _canRotate = !_canRotate;
    }

    public void LookAtEnemy(Transform target)
    {
        _rigidbody.transform.LookAt(target);
    }

    private IEnumerator MobileMove()
    {
        while (_variableJoystick != null)
        {
            float mobileSpeed = _moveSpeed * 12.7f;
            _direction = Vector3.forward * _variableJoystick.Vertical + Vector3.right * _variableJoystick.Horizontal;
            _rigidbody.AddForce(_direction * mobileSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            MobileLookAt();

            yield return null;
        }
    }

    private void MobileLookAt()
    {
        if (_canRotate)
        {
            Vector3 direction = _rigidbody.velocity;
            direction.y = 0;
            Vector2 turn = new Vector2(_variableJoystick.Horizontal, _variableJoystick.Vertical);

            if (turn.sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
            else
                _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private Vector3 GetCameraRight(Camera camera)
    {
        Vector3 forward = camera.transform.right;
        forward.y = 0;

        return forward.normalized;
    }

    private Vector3 GetCameraForward(Camera camera)
    {
        Vector3 right = camera.transform.forward;
        right.y = 0;

        return right.normalized;
    }

    private void DekstopLookAt()
    {
        if (_canRotate)
        {
            Vector3 direction = _rigidbody.velocity;
            direction.y = 0;

            if (_move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
            else
                _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private IEnumerator DekstopMove()
    {
        while (_playerInputSystem != null)
        {
            _direction += _move.ReadValue<Vector2>().x * GetCameraRight(_camera) * _moveSpeed;
            _direction += _move.ReadValue<Vector2>().y * GetCameraForward(_camera) * _moveSpeed;

            _rigidbody.AddForce(_direction, ForceMode.Impulse);
            _direction = Vector3.zero;

            if (_rigidbody.velocity.y < 0f)
                _rigidbody.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

            Vector3 horizontalVelocity = _rigidbody.velocity;
            horizontalVelocity.y = 0;

            if (horizontalVelocity.sqrMagnitude > _maxMoveSpeed * _maxMoveSpeed)
                _rigidbody.velocity = horizontalVelocity.normalized * _maxMoveSpeed + Vector3.up * _rigidbody.velocity.y;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            DekstopLookAt();

            yield return null;
        }
    }
}