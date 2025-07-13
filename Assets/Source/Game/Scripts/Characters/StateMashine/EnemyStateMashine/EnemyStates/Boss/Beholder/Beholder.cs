using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class Beholder : Boss
    {
        [SerializeField] private Bullet _bullet;
        [SerializeField] private Transform _baseShotPoint;
        [SerializeField] private Transform[] _shotPoints;
        [SerializeField] private DragonFlame _dragonFlame;

        public Transform BaseShotPoint => _baseShotPoint;
        public Transform[] ShotPoints => _shotPoints;
        public Bullet Bullet => _bullet;
        public DragonFlame DragonFlame => _dragonFlame;
    }
}