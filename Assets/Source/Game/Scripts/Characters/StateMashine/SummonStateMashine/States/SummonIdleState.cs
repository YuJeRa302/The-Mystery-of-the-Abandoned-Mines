using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SummonIdleState : State
{
    private Player _player;
    private Summon _summon;
    private Dictionary<float, Enemy> _enemies = new Dictionary<float, Enemy>();

    public SummonIdleState(StateMashine stateMashine, Player player, Summon summon) : base(stateMashine)
    {
        _player = player;
        _summon = summon;
    }

    public override void EnterState()
    {
        base.EnterState();
        SetIdleState();
    }

    public override void UpdateState()
    {
        if (FindEnemy(out Enemy target))
        {
            _summon.SetTarget(target);
            _stateMashine.SetState<SummonMoveState>();
        }

        Vector3 directionToPlayer = _summon.transform.position - _player.transform.position;
        float distance = directionToPlayer.magnitude;

        if (distance > _summon.DistanceToTarget)
        {
            _stateMashine.SetState<SummonMoveState>();
        }
    }

    private bool FindEnemy(out Enemy target)
    {
        _enemies.Clear();
        var colliders = Physics.OverlapSphere(_summon.transform.position, _summon.SearchRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out Enemy enemy))
            {
                float distanceToTarget = Vector3.Distance(enemy.transform.position, _summon.transform.position);

                if (distanceToTarget <= _summon.SearchRadius)
                    _enemies.Add(distanceToTarget, enemy);
            }
        }

        if (_enemies.Count == 0)
        {
           target = null;
        }
        else
        {
            target = _enemies.OrderBy(distance => distance.Key).First().Value;

            if (target != null && target.isActiveAndEnabled == true)
            {
                return true;
            }
        }

        return (target != null && target.isActiveAndEnabled == true);
    }
}