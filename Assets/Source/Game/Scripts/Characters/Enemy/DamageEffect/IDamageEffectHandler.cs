using Assets.Source.Game.Scripts.Enums;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Characters
{
    public interface IDamageEffectHandler
    {
        public void ApplayDamageEffect(DamageSource damageSource,
            Dictionary<TypeDamageParameter, float> extractDamage);
    }
}