using Assets.Source.Game.Scripts;
using UnityEngine;

public class Beholder : Boss
{
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Transform _baseShotPoint;
    [SerializeField] private Transform[] _shotPoints;
    [SerializeField] private DragonFlame _dragonFlame;

    public Transform BaseShotPoint => _baseShotPoint;
    public Transform[] ShotPoints => _shotPoints;
    public Bullet Bullet => _bullet;
    public Pool Pool => base.Pool;
    public DragonFlame DragonFlame => _dragonFlame;

    public override void Initialize(Player player, int lvlRoom, EnemyData data, int tire)
    {
        base.Initialize(player, lvlRoom, data, tire);
    }
}