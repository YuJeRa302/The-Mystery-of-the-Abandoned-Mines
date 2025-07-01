using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using YG;

public class PlayerMovement : IDisposable
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly GamePauseService _gamePauseService;
    private readonly Player _player;

    private Camera _camera;
    private VariableJoystick _variableJoystick;
    private PlayerInput _playerInputSystem;
    private Vector3 _direction;
    private Rigidbody _rigidbody;
    private InputAction _move;
    private Coroutine _movement;
    private bool _canRotate = true;
    private bool _canMove = true;

    public PlayerMovement(
        Camera camera,
        VariableJoystick variableJoystick,
        Rigidbody rigidbody,
        Player player,
        ICoroutineRunner coroutineRunner,
        GamePauseService gamePauseService)
    {
        _player = player;
        _gamePauseService = gamePauseService;
        _coroutineRunner = coroutineRunner;
        _rigidbody = rigidbody;
        _camera = camera;

        if (YG2.envir.isDesktop)
        {
            CreateInputSystem();
            variableJoystick.gameObject.SetActive(false);
            _move = _playerInputSystem.Player.Move;
            _movement = _coroutineRunner.StartCoroutine(DesktopMove());
        }
        else
        {
            _variableJoystick = variableJoystick;
            _movement = _coroutineRunner.StartCoroutine(MobileMove());
        }

        _variableJoystick = variableJoystick;
        _movement = _coroutineRunner.StartCoroutine(MobileMove());

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

    public void DisableMovment()
    {
        _canMove = false;

        if (_playerInputSystem != null)
            _playerInputSystem.Disable();

        if (_movement != null)
            _coroutineRunner.StopCoroutine(_movement);
    }

    public void ChangeRotate(bool canRotate)
    {
        _canRotate = canRotate;
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
        _gamePauseService.GamePaused += OnPauseGame;
        _gamePauseService.GameResumed += OnResumeGame;
    }

    private void RemoveListeners()
    {
        _gamePauseService.GamePaused -= OnPauseGame;
        _gamePauseService.GameResumed -= OnResumeGame;
    }

    private void OnPauseGame(bool state)
    {
        if (_movement != null)
            _coroutineRunner.StopCoroutine(_movement);
    }

    private void OnResumeGame(bool state)
    {
        if (_movement != null)
            _coroutineRunner.StopCoroutine(_movement);

        if (_canMove) 
        {
            if (YG2.envir.isDesktop)
                _movement = _coroutineRunner.StartCoroutine(DesktopMove());
            else
                _movement = _coroutineRunner.StartCoroutine(MobileMove());
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

    private IEnumerator MobileMove()
    {
        while (_variableJoystick != null)
        {
            Vector2 joystickInput = new Vector2(_variableJoystick.Horizontal, _variableJoystick.Vertical);
            _direction += joystickInput.x * GetCameraRight(_camera) * _player.MoveSpeed;
            _direction += joystickInput.y * GetCameraForward(_camera) * _player.MoveSpeed;
            _rigidbody.AddForce(_direction, ForceMode.Impulse);
            _direction = Vector3.zero;

            if (_rigidbody.velocity.y < 0f)
                _rigidbody.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

            Vector3 horizontalVelocity = _rigidbody.velocity;
            horizontalVelocity.y = 0;

            if (horizontalVelocity.sqrMagnitude > _player.MaxMoveSpeed * _player.MaxMoveSpeed)
                _rigidbody.velocity = horizontalVelocity.normalized * _player.MaxMoveSpeed + Vector3.up * _rigidbody.velocity.y;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            MobileLookAt();

            yield return null;
        }
    }

    private IEnumerator DesktopMove()
    {
        while (_playerInputSystem != null)
        {
            _direction += _move.ReadValue<Vector2>().x * GetCameraRight(_camera) * _player.MoveSpeed;
            _direction += _move.ReadValue<Vector2>().y * GetCameraForward(_camera) * _player.MoveSpeed;
            _rigidbody.AddForce(_direction, ForceMode.Impulse);
            _direction = Vector3.zero;

            if (_rigidbody.velocity.y < 0f)
                _rigidbody.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

            Vector3 horizontalVelocity = _rigidbody.velocity;
            horizontalVelocity.y = 0;

            if (horizontalVelocity.sqrMagnitude > _player.MaxMoveSpeed * _player.MaxMoveSpeed)
                _rigidbody.velocity = horizontalVelocity.normalized * _player.MaxMoveSpeed + Vector3.up * _rigidbody.velocity.y;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            DekstopLookAt();

            yield return null;
        }
    }
}