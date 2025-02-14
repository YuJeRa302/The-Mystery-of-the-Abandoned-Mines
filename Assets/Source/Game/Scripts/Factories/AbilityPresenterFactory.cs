using System;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AbilityPresenterFactory
    {
        private readonly IGameLoopService _gameLoopService;
        private readonly ICoroutineRunner _coroutineRunner;

        public AbilityPresenterFactory(IGameLoopService gameLoopService, ICoroutineRunner coroutineRunner)
        {
            _gameLoopService = gameLoopService ?? throw new ArgumentNullException(nameof(gameLoopService));
            _coroutineRunner = coroutineRunner ?? throw new ArgumentNullException(nameof(coroutineRunner));
        }

        public AttackAbilityPresenter CreateAttackAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            Transform throwPoint,
            ParticleSystem particleSystem,
            Spell spell)
        {
            AttackAbilityPresenter attackAbilityPresenter = new AttackAbilityPresenter(ability, abilityView, player, throwPoint, particleSystem, _gameLoopService, _coroutineRunner, spell);
            return attackAbilityPresenter;
        }

        public AmplifierAbilityPresenter CreateAmplifierAbilityPresenter(Ability ability, AbilityView abilityView, ParticleSystem particleSystem)
        {
            ValidateNotNull(ability, abilityView, particleSystem);

            return CreatePresenter<AmplifierAbilityPresenter>(
                ability,
                abilityView,
                particleSystem,
                _gameLoopService);
        }

        public SummonAbillityPresenter CreateSummonAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Transform spawnPoint,
            Player player,
            Summon summonPrefab,
            Pool pool)
        {
            ValidateNotNull(ability, abilityView, spawnPoint, player, summonPrefab, pool);

            return CreatePresenter<SummonAbillityPresenter>(
                ability,
                abilityView,
                spawnPoint,
                player,
                _gameLoopService,
                _coroutineRunner,
                summonPrefab,
                pool);
        }

        public ThrowAxeAbilityPresenter CreateThrowAxePresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            AxemMssile axemMssile)
        {
            ValidateNotNull(ability, abilityView, player, axemMssile);

            return CreatePresenter<ThrowAxeAbilityPresenter>(
                ability,
                abilityView,
                player.ShotPoint,
                player,
                _gameLoopService,
                _coroutineRunner,
                axemMssile,
                player.Pool);
        }

        public JerkFrontAbillityPresenter CreateJerkFrontAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle abilityEffect)
        {
            ValidateNotNull(ability, abilityView, player, abilityEffect);

            return CreatePresenter<JerkFrontAbillityPresenter>(
                ability,
                abilityView,
                player,
                _gameLoopService,
                _coroutineRunner,
                abilityEffect);
        }

        public RageAbillityPresenter CreateRageAbilityPresenter(Ability ability, AbilityView abilityView, Player player, int boostDamage, float boostMoveSpeed, int boostArmor, PoolParticle abilityEffect)
        {
            ValidateNotNull(ability, abilityView, player, abilityEffect);

            return CreatePresenter<RageAbillityPresenter>(
                ability,
                abilityView,
                player,
                boostDamage,
                boostMoveSpeed,
                boostArmor,
                _gameLoopService,
                _coroutineRunner,
                abilityEffect);
        }

        public EpiphanyAbilityPresenter CreateEpiphanyAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle abilityEffect)
        {
            ValidateNotNull(ability, abilityView, player, abilityEffect);

            return CreatePresenter<EpiphanyAbilityPresenter>(
                ability,
                abilityView,
                player,
                _gameLoopService,
                _coroutineRunner,
                abilityEffect);
        }

        public ShildUpAbilityPresenter CreateShieldUpAbility(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            ValidateNotNull(ability, abilityView, player, poolParticle);

            return CreatePresenter<ShildUpAbilityPresenter>(
                ability,
                abilityView,
                player,
                _gameLoopService,
                _coroutineRunner,
                poolParticle);
        }

        public SoulExplosionAbilityPresenter CreateSoulExplosionAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            ValidateNotNull(ability, abilityView, player, poolParticle);

            return CreatePresenter<SoulExplosionAbilityPresenter>(
                ability,
                abilityView,
                player,
                _gameLoopService,
                _coroutineRunner,
                poolParticle);
        }

        public DarkPactAbilityPresenter CreateDarkPactAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            ValidateNotNull(ability, abilityView, player, poolParticle);

            return CreatePresenter<DarkPactAbilityPresenter>(
                ability,
                abilityView,
                player,
                _gameLoopService,
                _coroutineRunner,
                poolParticle);
        }

        public StunningBlowAbilityPresenter CreateStunningBlowAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            ValidateNotNull(ability, abilityView, player, poolParticle);

            return CreatePresenter<StunningBlowAbilityPresenter>(
                ability,
                abilityView,
                player,
                _gameLoopService,
                _coroutineRunner,
                poolParticle);
        }

        private T CreatePresenter<T>(params object[] parameters) where T : class
        {
            var constructor = typeof(T).GetConstructor(parameters.Select(p => p.GetType()).ToArray());

            return constructor?.Invoke(parameters) as T;
        }

        private void ValidateNotNull(params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj == null)
                    throw new ArgumentNullException("One or more arguments provided to the factory method are null.");
            }
        }

        public GlobularLightningPresenter CreateGlobularLightningPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            GlobularLightningPresenter globularLightningPresenter = new(
                ability,
                abilityView,
                player,
                particleSystem,
                _gameLoopService,
                _coroutineRunner,
                spell);

            return globularLightningPresenter;
        }

        public FirestormPresenter CreateFirestormPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            FirestormPresenter firestormPresenter = new FirestormPresenter(ability, abilityView, player, player.ThrowAbilityPoint, particleSystem, _gameLoopService, _coroutineRunner, spell);

            return firestormPresenter;
        }

        public MetiorSowerPresenter CreateMetiorSowerPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            MetiorSowerPresenter metiorSower = new MetiorSowerPresenter(ability, abilityView, player, particleSystem, _gameLoopService, _coroutineRunner, spell);
            return metiorSower;
        }
    }
}