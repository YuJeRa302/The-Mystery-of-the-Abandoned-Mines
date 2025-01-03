using Assets.Source.Game.Scripts;
using UnityEngine;

public class Beholder : Boss
{
    [SerializeField] private Bullet _bullet;
    [SerializeField] private Pool _pool;
    [SerializeField] private Transform _baseShotPoint;
    [SerializeField] private Transform[] _shotPoints;

    public Transform BaseShotPoint => _baseShotPoint;
    public Transform[] ShotPoints => _shotPoints;
    public Bullet Bullet => _bullet;
    public Pool Pool => _pool;
}