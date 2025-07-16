using Assets.Source.Game.Scripts.Characters;

namespace Assets.Source.Game.Scripts.Traps
{
    public class DamageZone : Trap
    {
        protected override void ApplyDamage(Player player)
        {
            player.TakeDamage(Damage);
        }
    }
}