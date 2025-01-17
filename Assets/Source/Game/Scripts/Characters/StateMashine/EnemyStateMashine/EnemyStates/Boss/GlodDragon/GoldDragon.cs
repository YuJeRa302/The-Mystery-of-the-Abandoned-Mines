using Assets.Source.Game.Scripts;
using UnityEngine;

public class GoldDragon : Boss
{
    [SerializeField] private DragonFlame _dragonFlame;

    public DragonFlame DragonFlame => _dragonFlame;
}