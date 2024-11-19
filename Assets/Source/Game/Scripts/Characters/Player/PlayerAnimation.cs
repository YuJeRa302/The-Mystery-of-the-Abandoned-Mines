using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private MovmetPlayer _playerMovment;
        [SerializeField] private Player _player;
        [SerializeField] private PlayerAttacker _playerAttacker;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private float _maxSpeed;
        private HashAnimationPlayer _animationPlayer = new HashAnimationPlayer();

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _maxSpeed = _playerMovment.MaxMoveSpeed;
        }

        private void OnEnable()
        {
            _playerAttacker.Attacked += OnAttackAnimation;
        }

        private void OnDisable()
        {
            _playerAttacker.Attacked -= OnAttackAnimation;
        }

        private void Update()
        {
            _animator.SetFloat(_animationPlayer.MoveAnimation, _rigidbody.velocity.magnitude / _maxSpeed);
        }

        private void OnAttackAnimation()
        {
            _animator.SetTrigger(_animationPlayer.AttackAnimation);
        }
    }
}