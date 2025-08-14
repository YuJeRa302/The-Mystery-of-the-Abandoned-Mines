using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using System.Collections.Generic;

public interface IDamageEffectHandler
{
    public void ApplayDamageEffect(DamageSource damageSource, 
        Dictionary<TypeDamageParameter, float> extractDamage);
}