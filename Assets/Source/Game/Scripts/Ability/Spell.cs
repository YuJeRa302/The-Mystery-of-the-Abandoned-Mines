using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class Spell : MonoBehaviour
    {
        [SerializeField] private Transform _effectContainer;
        [SerializeField] private float _findEnemyRange = 4f;

        private ParticleSystem _abilityEffect;
        private float _spellLifeTime;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _findEnemyRange);
        }

        public void Initialize(ParticleSystem particleSystem, float currentDuration)
        {
            if (_abilityEffect != null)
            {
                if (_abilityEffect.gameObject != null)
                    Destroy(_abilityEffect);
            }

            _spellLifeTime = currentDuration;
            CreateEffect(particleSystem);
            Destroy(_abilityEffect.gameObject, _spellLifeTime);
            Destroy(gameObject, _spellLifeTime);
        }

        public bool TryFindEnemy(out Enemy enemy)
        {
            Collider[] coliderEnemy = Physics.OverlapSphere(transform.position, _findEnemyRange);

            foreach (Collider collider in coliderEnemy)
            {
                if (collider.TryGetComponent(out enemy))
                    return true;
            }

            enemy = null;
            return false;
        }

        public void DestoySpell()
        {
            Destroy(gameObject);
        }

        private void CreateEffect(ParticleSystem particleSystem)
        {
            _abilityEffect = Instantiate(particleSystem, transform);
            _abilityEffect.Play();
        }
    }
}