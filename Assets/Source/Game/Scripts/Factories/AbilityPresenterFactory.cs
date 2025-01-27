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
            _gameLoopService = gameLoopService;
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
            AttackAbilityPresenter attackAbilityPresenter = new (
                ability,
                abilityView,
                player,
                throwPoint,
                particleSystem,
                _gameLoopService,
                _coroutineRunner,
                spell);

            return attackAbilityPresenter;
        }

        public AmplifierAbilityPresenter CreateAmplifierAbilityPresenter(Ability ability, AbilityView abilityView, ParticleSystem particleSystem) 
        {
            AmplifierAbilityPresenter amplifierAbilityPresenter = new (ability, abilityView, particleSystem, _gameLoopService);
            return amplifierAbilityPresenter;
        }

        public SummonAbillityPresenter CreateSummonAbilityPresenter(
            Ability ability, 
            AbilityView abilityView,
            Transform spawnPoint,
            Player player,
            Summon summonPrefab,
            Pool pool)
        {
            SummonAbillityPresenter summonAbillityPresenter = new SummonAbillityPresenter(ability, abilityView, spawnPoint, player, _gameLoopService, _coroutineRunner, summonPrefab, pool);
            return summonAbillityPresenter;
        }

        public ThrowAxeAbilityPresenter CreateThrowAxePresenter(
            Ability ability,
            AbilityView abilityView,
            Player player,
            AxemMssile axemMssile)
        {
            ThrowAxeAbilityPresenter throwAxeAbilityPresenter = new ThrowAxeAbilityPresenter(ability, abilityView, player.ShotPoint, player, _gameLoopService, _coroutineRunner, axemMssile, player.Pool);
            return throwAxeAbilityPresenter;
        }

        public JerkFrontAbillityPresenter CreateJerkFrontAnillityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle abilityEffect)
        {
            JerkFrontAbillityPresenter jerkFrontAbillityPresenter = new JerkFrontAbillityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, abilityEffect);
            return jerkFrontAbillityPresenter;
        }

        public RageAbillityPresenter CreateRageAbilityPresenter(Ability ability, AbilityView abilityView, Player player, int boostDamage, float boostMoveSpeed, int boosArmor, PoolParticle abilityEffect)
        {
            RageAbillityPresenter rageAbillityPresenter = new RageAbillityPresenter(ability, abilityView, player, boostDamage, boostMoveSpeed, boosArmor, _gameLoopService, _coroutineRunner, abilityEffect);
            return rageAbillityPresenter;
        }

        public EpiphanyAbilityPresenter CreateEpiphanyAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle abilityEffect)
        {
            EpiphanyAbilityPresenter epiphanyAbilityPresenter = new EpiphanyAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, abilityEffect);
            return epiphanyAbilityPresenter;
        }

        public ShildUpAbilityPresenter CreateShieldUpAbility(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            ShildUpAbilityPresenter shildUpAbility = new ShildUpAbilityPresenter(ability,abilityView, player, _gameLoopService, _coroutineRunner, poolParticle);
            return shildUpAbility;
        }

        public SoulExplosionAbilityPresenter CreateSoulExplosionAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            SoulExplosionAbilityPresenter soulExplosionAbilityPresenter = new SoulExplosionAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, poolParticle);
            return soulExplosionAbilityPresenter;
        }

        public DarkPactAbilityPresenter CreateDarkPactAbilityPresenter(Ability ability, AbilityView abilityView, Player player, PoolParticle poolParticle)
        {
            DarkPactAbilityPresenter darkPactAbilityPresenter = new DarkPactAbilityPresenter(ability, abilityView, player, _gameLoopService, _coroutineRunner, poolParticle);
            return darkPactAbilityPresenter;
        }
    }
}