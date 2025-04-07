using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : IDisposable
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IGameLoopService _gameLoopService;
    private readonly float _amplifierJoystick = 12.7f;

    private float _moveSpeed;
    private float _maxMoveSpeed;
    private Camera _camera;
    private VariableJoystick _variableJoystick;
    private PlayerInput _playerInputSystem;
    private Vector3 _direction;
    private Rigidbody _rigidbody;
    private InputAction _move;
    private Coroutine _movement;
    private bool _isModile = false;
    private bool _canRotate = true;
    private bool _isDecstop = true; //

    public float MaxMoveSpeed => _maxMoveSpeed;
    public float MoveSpeed => _moveSpeed;

    public PlayerMovement(
        Camera camera,
        VariableJoystick variableJoystick,
        Rigidbody rigidbody,
        float moveSpeed,
        ICoroutineRunner coroutineRunner,
        IGameLoopService gameLoopService)
    {
        _gameLoopService = gameLoopService;
        _coroutineRunner = coroutineRunner;
        _moveSpeed = moveSpeed;
        _rigidbody = rigidbody;
        _camera = camera;
        _variableJoystick = variableJoystick;
        CreateInputSystem();
        _move = _playerInputSystem.Player.Move;
        _maxMoveSpeed = _moveSpeed * 2f;
        _movement = _coroutineRunner.StartCoroutine(DesktopMove());
        AddListeners();
    }

    public void Dispose()
    {
        if (_playerInputSystem != null)
            _playerInputSystem.Disable();

        if (_movement != null)
            _coroutineRunner.StopCoroutine(_movement);

        RemoveListeners();
        GC.SuppressFinalize(this);
    }

    public void ChangeMoveSpeed(float value)
    {
        _moveSpeed = value;
        _maxMoveSpeed = _moveSpeed * 2f;
    }

    public void ChangeRotate()
    {
        _canRotate = !_canRotate;
    }

    public void LookAtEnemy(Transform target)
    {
        _rigidbody.transform.LookAt(new Vector3(target.position.x, 0 , target.position.z));
    }

    private void CreateInputSystem() 
    {
        if (_playerInputSystem == null)
        {
            _playerInputSystem = new PlayerInput();
            _playerInputSystem.Enable();
        }
    }

    private void AddListeners()
    {
        _gameLoopService.GamePaused += OnPauseGame;
        _gameLoopService.GameResumed += OnResumeGame;
    }

    private void RemoveListeners()
    {
        _gameLoopService.GamePaused -= OnPauseGame;
        _gameLoopService.GameResumed -= OnResumeGame;
    }

    private void OnPauseGame()
    {
        if (_movement != null)
            _coroutineRunner.StopCoroutine(_movement);
    }

    private void OnResumeGame()
    {
        if (_movement != null)
            _coroutineRunner.StopCoroutine(_movement);

        _movement = _coroutineRunner.StartCoroutine(DesktopMove());
    }

    private IEnumerator MobileMove()
    {
        while (_variableJoystick != null)
        {
            float mobileSpeed = _moveSpeed * _amplifierJoystick;
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

        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
    }

    private IEnumerator DesktopMove()
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