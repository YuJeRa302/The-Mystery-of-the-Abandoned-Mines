using Assets.Source.Game.Scripts;
using UnityEngine;

public class GoldDragon : Boss
{
    [SerializeField] private DragonFlame _dragonFlame;
    [SerializeField] private DragonSpell _dragonSpell;
    [SerializeField] private float _damageSpell;

    public DragonFlame DragonFlame => _dragonFlame;
    public DragonSpell DragonSpell => _dragonSpell;
    public float DamageSpell => _damageSpell;
}