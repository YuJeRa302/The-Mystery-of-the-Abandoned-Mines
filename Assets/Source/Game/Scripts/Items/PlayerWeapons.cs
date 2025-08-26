using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System;
using UniRx;

namespace Assets.Source.Game.Scripts.Items
{
    public class PlayerWeapons : IDisposable
    {
        private Player _player;
        private WeaponData _weapon;
        private WeaponPrefab _weponPrefab;
        private Pool _pool;
        private PoolParticle _critParticle;
        private PoolParticle _vampirismParticle;
        private CompositeDisposable _disposables = new();

        public PlayerWeapons(Player player, WeaponData weaponData, Pool pool, PoolParticle critParticle, PoolParticle vampiticmParticle)
        {
            _player = player;
            _weapon = weaponData;
            _weponPrefab = _weapon.WeaponPrefab;
            _pool = pool;
            _critParticle = critParticle;
            _vampirismParticle = vampiticmParticle;

            CreateWeaponView();
            AddListener();
        }

        public void Dispose()
        {
            if (_disposables != null)
                _disposables.Dispose();
        }

        public WeaponData GetWeaponData()
        {
            return _weapon;
        }

        public void ChangeTrailEffect()
        {
            if (_weponPrefab.KickEffect != null)
            {
                PoolParticle particle;

                if (_pool.TryPoolObject(_weponPrefab.KickEffect.gameObject, out PoolObject poolParticle))
                {
                    particle = poolParticle as PoolParticle;
                    particle.gameObject.SetActive(true);
                }
                else
                {
                    particle = UnityEngine.Object.Instantiate(_weponPrefab.KickEffect, _player.transform);
                    _pool.InstantiatePoolObject(particle, _weponPrefab.KickEffect.name);
                }
            }
        }

        private void InstantiateCritEffect()
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(_critParticle.gameObject, out PoolObject poolParticle))
            {
                particle = poolParticle as PoolParticle;
                particle.transform.position = _player.transform.position;
                particle.transform.rotation = _player.transform.rotation;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = UnityEngine.Object.Instantiate(_critParticle,
                    _player.transform.position, _player.transform.rotation);
                _pool.InstantiatePoolObject(particle, _vampirismParticle.name);
            }
        }

        private void InstantiateHealingEffect()
        {
            PoolParticle particle;

            if (_pool.TryPoolObject(_vampirismParticle.gameObject, out PoolObject poolParticle))
            {
                particle = poolParticle as PoolParticle;
                particle.transform.position = _player.transform.position;
                particle.gameObject.SetActive(true);
            }
            else
            {
                particle = UnityEngine.Object.Instantiate(_vampirismParticle, _player.transform);
                _pool.InstantiatePoolObject(particle, _critParticle.name);
            }
        }

        private void CreateWeaponView()
        {
            UnityEngine.Object.Instantiate(_weapon.WeaponPrefab, _player.WeaponPoint);

            if (_weapon.TypePlayerClass == TypePlayerClass.Paladin)
            {
                PaladinWeaponData paladinWeaponData = _weapon as PaladinWeaponData;
                UnityEngine.Object.Instantiate(paladinWeaponData.AdditionalWeapon, _player.AdditionalWeaponPoint);
            }
        }

        private void AddListener()
        {
            CardDeck.MessageBroker
                .Receive<M_CritAttack>()
                .Subscribe(m => InstantiateCritEffect())
                .AddTo(_disposables);

            CardDeck.MessageBroker
                .Receive<M_HealedVampirism>()
                .Subscribe(m => InstantiateHealingEffect())
                .AddTo(_disposables);
        }
    }
}