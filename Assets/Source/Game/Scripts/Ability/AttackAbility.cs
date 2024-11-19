using System.Collections;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AttackAbility : Ability
    {
        private readonly float _delayAttack = 0.3f;
        private readonly float _blastSpeed = 0.2f;

        private Vector3 _direction;
        private Spell _spell;

        protected override void ApplyAbility()
        {
            base.ApplyAbility();

            if (TypeAttackAbility == TypeAttackAbility.AoEAbility)
                CreateAoEAbility();

            if (TypeAttackAbility == TypeAttackAbility.ProjectileAbility)
                ThrowAbilityBlast();

            if (DealDamage() != null)
                StopCoroutine(DealDamage());

            StartCoroutine(DealDamage());
        }

        private void ThrowAbilityBlast()
        {
            _spell = Instantiate(
                Spell,
                new Vector3(ThrowPoint.transform.position.x, ThrowPoint.transform.position.y, ThrowPoint.transform.position.z),
                Quaternion.identity);

            _direction = ThrowPoint.transform.forward;
            _spell.Initialize(ParticleSystem, this);
            StartCoroutine(ThrowingBlast());
        }

        private void CreateAoEAbility()
        {
            _spell = Instantiate(
                Spell,
                new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z),
                Quaternion.identity);

            _spell.Initialize(ParticleSystem, this);
        }

        private IEnumerator DealDamage()
        {
            while (IsAbilityEnded == false)
            {
                yield return new WaitForSeconds(_delayAttack);

                if (_spell != null) 
                {
                    if (_spell.TryFindEnemy(out Enemy enemy))
                        enemy.TakeDamage(CurrentAbilityValue);
                }
            }
        }

        private IEnumerator ThrowingBlast()
        {
            while (IsAbilityEnded == false)
            {
                if (_spell != null)
                    _spell.transform.Translate(_direction * _blastSpeed);

                yield return null;
            }
        }
    }
}