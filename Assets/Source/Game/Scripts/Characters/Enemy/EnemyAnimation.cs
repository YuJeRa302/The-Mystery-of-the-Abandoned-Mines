using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private EnemyStateMashineExample _enemyStateMashine;

    private Animator _animator;
    private HashAnimationEnemy _animationEnemy = new HashAnimationEnemy();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _enemyStateMashine.MashineInitialized += AddAnimationAction;
    }

    private void OnDisable()
    {
        _enemyStateMashine.MashineInitialized -= AddAnimationAction;

        foreach (var events in _enemyStateMashine.MashineStates)
        {
            events.Value.Attacking -= OnAttack;
            events.Value.Moving -= OnMove;
            events.Value.TakedDamage -= OnTakeDamage;
            events.Value.PlayerLose -= OnWinGame;
        }
    }

    private void AddAnimationAction()
    {
        foreach (var events in _enemyStateMashine.MashineStates)
        {
            events.Value.Attacking += OnAttack;
            events.Value.Moving += OnMove;
            events.Value.TakedDamage += OnTakeDamage;
            events.Value.PlayerLose += OnWinGame;
        }
    }

    private void OnMove() => _animator.SetTrigger(_animationEnemy.MoveAnimation);

    private void OnAttack() => _animator.SetTrigger(_animationEnemy.AttackAnimation);

    private void OnTakeDamage() => _animator.SetTrigger(_animationEnemy.TakeDamageAnimation);

    private void OnWinGame() => _animator.SetTrigger(_animationEnemy.WinDanceAnimation);
}