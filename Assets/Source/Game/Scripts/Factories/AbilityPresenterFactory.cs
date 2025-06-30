using System;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class AbilityPresenterFactory
    {

        private readonly IGameLoopService _gameLoopService;
        private readonly ICoroutineRunner _coroutineRunner;

        private GameLoopService _gameLoopService1;
        private GamePauseService _gamePauseService;

        public AbilityPresenterFactory(IGameLoopService gameLoopService, ICoroutineRunner coroutineRunner)
        {
            _gameLoopService = gameLoopService ?? throw new ArgumentNullException(nameof(gameLoopService));
            _coroutineRunner = coroutineRunner ?? throw new ArgumentNullException(nameof(coroutineRunner));
        }

        public AbilityPresenterFactory()
        {
        }

        public void InitService(GameLoopService gameLoopService, GamePauseService gamePauseService) 
        {
            _gameLoopService1 = gameLoopService;
            _gamePauseService = gamePauseService;
        }

        public AttackAbilityPresenter CreateAttackAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            Transform throwPoint,
            ParticleSystem particleSystem,
            Spell spell)
        {
            AttackAbilityPresenter attackAbilityPresenter = new AttackAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, spell, particleSystem);
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

        public SummonAbillityPresenter CreateSummonAbilityPresenter(
            Ability ability,
            AbilityView abilityView,
            Transform spawnPoint,
            Player player,
            Summon summonPrefab,
            Pool pool)
        {
            SummonAbillityPresenter summonAbillityPresenter = new SummonAbillityPresenter(ability, 
                abilityView, player, _gameLoopService, _coroutineRunner, summonPrefab);
            return summonAbillityPresenter;
        }

        public ThrowAxeAbilityPresenter CreateThrowAxePresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            AxemMssile axemMssile)
        {
            ThrowAxeAbilityPresenter throwAxeAbilityPresenter = new ThrowAxeAbilityPresenter(ability, 
                abilityView, player, _gameLoopService, _coroutineRunner, axemMssile);
            return throwAxeAbilityPresenter;
        }

        public JerkFrontAbillityPresenter CreateJerkFrontAbilityPresenter(Ability ability, 
            AbilityView abilityView, Player player, PoolParticle abilityEffect)
        {
            JerkFrontAbillityPresenter jerkFrontAbillityPresenter = new JerkFrontAbillityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, abilityEffect);
            return jerkFrontAbillityPresenter;
        }

        public RageAbillityPresenter CreateRageAbilityPresenter(Ability ability, 
            AbilityView abilityView, Player player, PoolParticle abilityEffect)
        {
            RageAbillityPresenter rageAbillityPresenter = new RageAbillityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, abilityEffect);
            return rageAbillityPresenter;
        }

        public EpiphanyAbilityPresenter CreateEpiphanyAbilityPresenter(Ability ability, 
            AbilityView abilityView, Player player, ParticleSystem abilityEffect, Spell spell)
        {
            EpiphanyAbilityPresenter epiphanyAbilityPresenter = new EpiphanyAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, abilityEffect, spell);
            return epiphanyAbilityPresenter;
        }

        public ShildUpAbilityPresenter CreateShieldUpAbility(Ability ability, 
            AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            ShildUpAbilityPresenter shildUpAbilityPresenter = new ShildUpAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, poolParticle);
            return shildUpAbilityPresenter;
        }

        public SoulExplosionAbilityPresenter CreateSoulExplosionAbilityPresenter(Ability ability, 
            AbilityView abilityView, Player player, ParticleSystem poolParticle, Spell spell)
        {
            SoulExplosionAbilityPresenter soulExplosionAbilityPresenter = new SoulExplosionAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, poolParticle, spell);
            return soulExplosionAbilityPresenter;
        }

        public DarkPactAbilityPresenter CreateDarkPactAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            DarkPactAbilityPresenter darkPactAbilityPresenter = new DarkPactAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, poolParticle);
            return darkPactAbilityPresenter;
        }

        public StunningBlowAbilityPresenter CreateStunningBlowAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            StunningBlowAbilityPresenter stunningBlowAbilityPresenter = new StunningBlowAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, poolParticle);
            return stunningBlowAbilityPresenter;
        }

        private T CreatePresenter<T>(params object[] parameters) where T : class
        {
            var constructor = typeof(T).GetConstructor(parameters.Select(p => p.GetType()).ToArray());

            return constructor?.Invoke(parameters) as T;
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
                _gameLoopService,
                _coroutineRunner,
                particleSystem,
                spell);

            return globularLightningPresenter;
        }

        public FirestormPresenter CreateFirestormPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            FirestormPresenter firestormPresenter = new FirestormPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);

            return firestormPresenter;
        }

        public MetiorSowerPresenter CreateMetiorSowerPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            MetiorSowerPresenter metiorSower = new MetiorSowerPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);
            return metiorSower;
        }

        public ElectricGuardPresenter CreateElectricGuardPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            ElectricGuardPresenter electricGuardPresenter = new ElectricGuardPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);
            return electricGuardPresenter;
        }

        public ThunderPresenter CreateThunderPresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            ThunderPresenter thunderPresenter = new ThunderPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);
            return thunderPresenter;
        }

        public DragonTracePresenter CreateDragonTracePresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            DragonTracePresenter dragonTracePresenter = new DragonTracePresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);
            return dragonTracePresenter;
        }

        public SnowfallPresenter CreateSnowfallPresenter(Ability ability,
            AbilityView abilityView, 
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            SnowfallPresenter snowfallPresenter = new SnowfallPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);
            return snowfallPresenter;
        }

        public IciAvalanchePresenter CreateIciAvalanchePresenter(Ability ability,
            AbilityView abilityView,
            Player player,
            ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            IciAvalanchePresenter iciAvalanchePresenter = new IciAvalanchePresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);
            return iciAvalanchePresenter;
        }

        public BuranPresenter CreateBuranPresenter(Ability ability,
            AbilityView abilityView,
            Player player, ParticleSystem particleSystem,
            LegendaryAbilitySpell spell)
        {
            BuranPresenter buranPresenter = new BuranPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, particleSystem, spell);
            return buranPresenter;
        }
    }
}