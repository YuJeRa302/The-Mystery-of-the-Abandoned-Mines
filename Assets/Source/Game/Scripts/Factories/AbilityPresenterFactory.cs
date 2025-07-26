using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.PoolSystem;
using Assets.Source.Game.Scripts.Services;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Factories
{
    public class AbilityPresenterFactory
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly GameLoopService _gameLoopService;
        private readonly GamePauseService _gamePauseService;

        public AbilityPresenterFactory(GameLoopService gameLoopService,
            GamePauseService gamePauseService,
            ICoroutineRunner coroutineRunner)
        {
            _gameLoopService = gameLoopService;
            _gamePauseService = gamePauseService;
            _coroutineRunner = coroutineRunner;
        }

        public AttackAbilityPresenter CreateAttackAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            Transform throwPoint,
            ParticleSystem particleSystem,
            Spell spell)
        {
            AttackAbilityPresenter attackAbilityPresenter = new AttackAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                spell,
                particleSystem);

            return attackAbilityPresenter;
        }

        public AmplifierAbilityPresenter CreateAmplifierAbilityPresenter(Ability ability,
            AbilityView abilityView, ParticleSystem particleSystem)
        {
            return CreatePresenter<AmplifierAbilityPresenter>(
                ability,
                abilityView,
                particleSystem,
                _gameLoopService);
        }

        public SummonAbilityPresenter CreateSummonAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            Summon summonPrefab)
        {
            SummonAbilityPresenter summonAbillityPresenter = new SummonAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                summonPrefab);

            return summonAbillityPresenter;
        }

        public ThrowAxeAbilityPresenter CreateThrowAxePresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            AxeMissile axeMssile)
        {
            ThrowAxeAbilityPresenter throwAxeAbilityPresenter = new ThrowAxeAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                axeMssile);

            return throwAxeAbilityPresenter;
        }

        public JerkFrontAbilityPresenter CreateJerkFrontAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            PoolParticle abilityEffect)
        {
            JerkFrontAbilityPresenter jerkFrontAbillityPresenter = new JerkFrontAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                abilityEffect);

            return jerkFrontAbillityPresenter;
        }

        public RageAbilityPresenter CreateRageAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            PoolParticle abilityEffect)
        {
            RageAbilityPresenter rageAbillityPresenter = new RageAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                abilityEffect);

            return rageAbillityPresenter;
        }

        public EpiphanyAbilityPresenter CreateEpiphanyAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem abilityEffect,
            Spell spell)
        {
            EpiphanyAbilityPresenter epiphanyAbilityPresenter = new EpiphanyAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                abilityEffect,
                spell);

            return epiphanyAbilityPresenter;
        }

        public ShieldUpAbilityPresenter CreateShieldUpAbility(
            Ability ability,
            AbilityView abilityView,
            Player player,
            PoolParticle poolParticle)
        {
            ShieldUpAbilityPresenter shildUpAbilityPresenter = new ShieldUpAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                poolParticle);

            return shildUpAbilityPresenter;
        }

        public SoulExplosionAbilityPresenter CreateSoulExplosionAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem poolParticle,
            Spell spell)
        {
            SoulExplosionAbilityPresenter soulExplosionAbilityPresenter = new SoulExplosionAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                poolParticle,
                spell);

            return soulExplosionAbilityPresenter;
        }

        public DarkPactAbilityPresenter CreateDarkPactAbilityPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            PoolParticle poolParticle)
        {
            DarkPactAbilityPresenter darkPactAbilityPresenter = new DarkPactAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                poolParticle);

            return darkPactAbilityPresenter;
        }

        public StunningBlowAbilityPresenter CreateStunningBlowAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            PoolParticle poolParticle)
        {
            StunningBlowAbilityPresenter stunningBlowAbilityPresenter = new StunningBlowAbilityPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                poolParticle);

            return stunningBlowAbilityPresenter;
        }

        public GlobularLightningPresenter CreateGlobularLightningPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            GlobularLightningPresenter globularLightningPresenter = new(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return globularLightningPresenter;
        }

        public FirestormPresenter CreateFirestormPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            FirestormPresenter firestormPresenter = new FirestormPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return firestormPresenter;
        }

        public MeteorShowerPresenter CreateMeteorShowerPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            MeteorShowerPresenter metiorSower = new MeteorShowerPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return metiorSower;
        }

        public ElectricGuardPresenter CreateElectricGuardPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            ElectricGuardPresenter electricGuardPresenter = new ElectricGuardPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return electricGuardPresenter;
        }

        public ThunderPresenter CreateThunderPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            ThunderPresenter thunderPresenter = new ThunderPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return thunderPresenter;
        }

        public DragonTracePresenter CreateDragonTracePresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            DragonTracePresenter dragonTracePresenter = new DragonTracePresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return dragonTracePresenter;
        }

        public SnowfallPresenter CreateSnowfallPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            SnowfallPresenter snowfallPresenter = new SnowfallPresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return snowfallPresenter;
        }

        public IciAvalanchePresenter CreateIciAvalanchePresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            IciAvalanchePresenter iciAvalanchePresenter = new IciAvalanchePresenter(
                ability,
                abilityView,
                player,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return iciAvalanchePresenter;
        }

        public BuranPresenter CreateBuranPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player, ParticleSystem particleSystem,
            LegendarySpell spell)
        {
            BuranPresenter buranPresenter = new BuranPresenter(
                ability,
                abilityView,
                player,
                particleSystem,
                spell,
                _gamePauseService,
                _gameLoopService,
                _coroutineRunner);

            return buranPresenter;
        }

        private T CreatePresenter<T>(params object[] parameters) where T : class
        {
            var constructor = typeof(T).GetConstructor(parameters.Select(p => p.GetType()).ToArray());

            return constructor?.Invoke(parameters) as T;
        }
    }
}