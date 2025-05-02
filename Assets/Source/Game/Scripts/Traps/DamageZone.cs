using Assets.Source.Game.Scripts;

public class DamageZone : Trap
{
    protected override void ApplyDamage(Player player)
    {
        player.TakeDamage(_damage);
    }
}