using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using Reflex.Extensions;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.Characters
{
    public class EnemyDamageHandler
    {
        private ICoroutineRunner _coroutineRunner;
        private Pool _pool;
        private List<PoolObject> _spawnedEffects = new ();
        private Transform _damageEffectContainer;
        private EnemyHealth _health;
        private Rigidbody _rigidbody;
        private Dictionary<TypeDamageParameter,float> _extractDamage = new();
        private Dictionary<TypeDamage, IDamageEffectHandler> _typeDamage;
        private CompositeDisposable _disposables = new();

        public EnemyDamageHandler(Pool pool, Transform effectContainer, Enemy enemy)
        {
            _pool = pool;
            _damageEffectContainer = effectContainer;
            _health = enemy.EnemyHealth;
            _rigidbody = enemy.Rigidbody;
            var container = SceneManager.GetActiveScene().GetSceneContainer();
            _coroutineRunner = container.Resolve<ICoroutineRunner>();

            _typeDamage = new Dictionary<TypeDamage, IDamageEffectHandler> 
            {
                { TypeDamage.BurningDamage, new BurningDamageHandler(_coroutineRunner, this)},
                { TypeDamage.RepulsiveDamage, new RepulsiveDamageHandler(_coroutineRunner, _rigidbody)},
                { TypeDamage.StunDamage, new StunDamageHandler(_coroutineRunner, this, enemy)},
                { TypeDamage.SlowedDamage, new SlowedDamageHandler(_coroutineRunner, this, enemy)},
            };

            MessageBroker.Default
                .Receive<M_CreateDamageParticle>()
                .Where(m => m.EnemyDamageHandler == this)
                .Subscribe(m => OnCreateDamageParticle(m.PoolParticle))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_DisableParticle>()
                .Where(m => m.EnemyDamageHandler == this)
                .Subscribe(m => OnDisableParticle(m.PoolParticle))
                .AddTo(_disposables);

            MessageBroker.Default
               .Receive<M_ApplyBurnDamage>()
               .Where(m => m.EnemyDamageHandler == this)
               .Subscribe(m => OnApplayDamage(m.Damage))
               .AddTo(_disposables);
        }

        public void CreateDamageEffect(DamageSource damageSource)
        {
            if(_typeDamage.TryGetValue(damageSource.TypeDamage, out IDamageEffectHandler effectHandler))
            {
                ExtractDamageParameters(damageSource);
                effectHandler.ApplayDamageEffect(damageSource, _extractDamage);

            }
        }

        public void Disable()
        {
            if (_disposables != null)
                _disposables.Dispose();

            foreach (var spawnedParticle in _spawnedEffects)
            {
                spawnedParticle.ReturnObjectPool();
            }
        }

        private void ExtractDamageParameters(DamageSource damageSource)
        {
            _extractDamage.Clear();

            foreach (var parameter in damageSource.DamageParameters)
            {
                if (_extractDamage.ContainsKey(parameter.TypeDamageParameter) == false)
                    _extractDamage.Add(parameter.TypeDamageParameter, parameter.Value);
                else
                    _extractDamage[parameter.TypeDamageParameter] = parameter.Value;
            }
        }

        private void OnApplayDamage(float damage) => _health.ApplyDamage(damage);

        private void OnCreateDamageParticle(PoolParticle poolParticle)
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(poolParticle.gameObject, out PoolObject pollParticle))
            {
                particle = pollParticle as PoolParticle;
                particle.transform.position = _damageEffectContainer.position;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = UnityEngine.Object.Instantiate(poolParticle, _damageEffectContainer);
                _pool.InstantiatePoolObject(particle, poolParticle.name);
                _spawnedEffects.Add(particle);
            }
        }

        private void OnDisableParticle(PoolParticle particle)
        {
            foreach (var spawnedParticle in _spawnedEffects)
            {
                if (particle.name == spawnedParticle.NameObject)
                    if (spawnedParticle.isActiveAndEnabled)
                        spawnedParticle.ReturnObjectPool();
            }
        }
    }
}