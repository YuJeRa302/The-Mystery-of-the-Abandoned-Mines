using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Factories;
using Assets.Source.Game.Scripts.GamePanels;
using Assets.Source.Game.Scripts.Menu;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.States;
using Assets.Source.Game.Scripts.Upgrades;
using Assets.Source.Game.Scripts.Views;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEditor.Playables;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class PlayerAbilityCaster : IDisposable
    {
        private readonly PlayerClassData _playerClassData;
        private readonly PersistentDataService _persistentDataService;
        private readonly AbilityEntitiesHolder _abilityEntitiesHolder;
        private readonly int _shiftIndex = 1;

        private IAbilityStrategy _abilityStrategy;
        private Player _player;
        private List<Ability> _abilities = new ();
        private List<Ability> _classAbilities = new ();
        private List<Ability> _legendaryAbilities = new ();
        private List<PassiveAbilityView> _passiveAbilityViews = new ();
        private AttributeData _abilityAttributeData;
        private AudioPlayer _audioPlayer;
        private AbilityFactory _abilityFactory;
        private AbilityView _abilityView;
        private Ability _ability;
        private ParticleSystem _particleSystem;
        private int _abilityDuration = 0;
        private int _abilityDamage = 0;
        private int _abilityCooldownReduction = 0;
        private int _currentAbilityLevel;
        private CompositeDisposable _disposables = new();

        public PlayerAbilityCaster(
            AbilityFactory abilityFactory,
            Player player,
            PersistentDataService persistentDataService,
            PlayerClassData playerClassData,
            AudioPlayer audioPlayer)
        {
            _abilityFactory = abilityFactory;
            _player = player;
            _persistentDataService = persistentDataService;
            _playerClassData = playerClassData;
            _audioPlayer = audioPlayer;
            _abilityEntitiesHolder = new AbilityEntitiesHolder(this);
        }

        public Player Player => _player;
        public Ability Ability => _ability;
        public AbilityView AbilityView => _abilityView;
        public AttributeData AttributeData => _abilityAttributeData;
        public ParticleSystem ParticleSystem => _particleSystem;
        public IAbilityStrategy IAbilityStrategy => _abilityStrategy;

        public void Initialize()
        {
            foreach (ClassAbilityData ability in _playerClassData.ClassAbilityDatas)
            {
                CreateClassAbility(ability);
            }

            AddListeners();
        }

        
        public void Dispose()
        {
            DestroyAbilities();

            if (_disposables != null)
                _disposables.Dispose();
        }

        public void CreateClassAbilityView(
            ClassAbilityData classAbilityData,
            ClassSkillButtonView classSkillButtonView,
            int currentLevel)
        {
            _ability = _abilityFactory.CreateClassSkill(classAbilityData, false, currentLevel - _shiftIndex);
            _abilityView = classSkillButtonView;
            _abilityAttributeData = classAbilityData;
            _abilityStrategy = classAbilityData.IAbilityStrategy;
            _abilityStrategy.Construct(_abilityEntitiesHolder);
            AbilityPresenter abilityPresenter = new(_abilityEntitiesHolder);
            _ability.AbilityUsed += OnAbilityUsed;
            _ability.AbilityEnded += OnAbilityEnded;
            _classAbilities.Add(_ability);
        }

        private void CreateAbilityView(AbilityView abilityView, ParticleSystem particleSystem)
        {
            _particleSystem = particleSystem;
            _abilityView = abilityView;

            _ability = _abilityFactory.CreateAbility(_abilityAttributeData as ActiveAbilityData,
                _currentAbilityLevel,
                _abilityCooldownReduction,
                _abilityDuration,
                _abilityDamage,
                true);

            _abilityStrategy = (_abilityAttributeData as ActiveAbilityData).IAbilityStrategy;
            _abilityStrategy.Construct(_abilityEntitiesHolder);
            AbilityPresenter abilityPresenter = new(_abilityEntitiesHolder);
            _ability.AbilityUsed += OnAbilityUsed;
            _ability.AbilityEnded += OnAbilityEnded;
            _abilities.Add(_ability);
        }

        public void CreateLegendaryAbilityView(
            AbilityView abilityView,
            ParticleSystem particleSystem,
            ActiveAbilityData abilityAttributeData)
        {
            _particleSystem = particleSystem;
            _abilityView = abilityView;
            _abilityAttributeData = abilityAttributeData;

            _ability = _abilityFactory.CreateLegendaryAbility(
                abilityAttributeData as LegendaryAbilityData,
                abilityAttributeData,
                _abilityCooldownReduction,
                _abilityDuration,
                _abilityDamage,
                true);

            _abilityStrategy = (abilityAttributeData as LegendaryAbilityData).IAbilityStrategy;
            _abilityStrategy.Construct(_abilityEntitiesHolder);
            AbilityPresenter abilityPresenter = new(_abilityEntitiesHolder);
            _ability.AbilityUsed += OnAbilityUsed;
            _ability.AbilityEnded += OnAbilityEnded;
            _legendaryAbilities.Add(_ability);
        }

        public void CreatePassiveAbilityView(PassiveAbilityView passiveAbilityView)
        {
            _passiveAbilityViews.Add(passiveAbilityView);
        }

        private void AddListeners()
        {
            CardDeck.MessageBroker
                .Receive<M_TakeAbility>()
                .Subscribe(m => TakeAbility(m.CardView))
                .AddTo(_disposables);

            CardDeck.MessageBroker
                .Receive<M_TakePassiveAbility>()
                .Subscribe(m => TakeAbility(m.CardView))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityDurationChange>()
                .Subscribe(m => AbilityDurationChanged(Convert.ToInt32(m.Value)))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityDamageChange>()
                .Subscribe(m => AbilityDamageChanged(Convert.ToInt32(m.Value)))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityCooldownReductionChange>()
                .Subscribe(m => AbilityCooldownReductionChanged(Convert.ToInt32(m.Value)))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityViewCreat>()
                .Subscribe(m => CreateAbilityView(
                m.AbilityView,
                m.ParticleSystem))
                .AddTo(_disposables);
        }

        private void AbilityDurationChanged(int value)
        {
            _abilityDuration = value;
        }

        private void AbilityDamageChanged(int value)
        {
            _abilityDamage = value;
        }

        private void AbilityCooldownReductionChanged(int value)
        {
            _abilityCooldownReduction = value;
        }

        private void TakeAbility(CardView cardView)
        {
            _abilityAttributeData = null;

            if (cardView.CardData.AttributeData as LegendaryAbilityData != null)
            {
                if (TrySetLegendaryAbility(
                    (cardView.CardData.AttributeData as LegendaryAbilityData).UpgradeType,
                    out Ability legendAbility))
                    CreateLegendaryAbility(legendAbility, legendAbility.AbilityAttribute);
            }

            if (cardView.CardData.AttributeData == null)
                return;

            if (cardView.CardData.AttributeData as PassiveAttributeData != null)
            {
                MessageBroker.Default.Publish(new M_PassiveAbilityTake(
                        cardView.CardData.AttributeData as PassiveAttributeData));
                cardView.CardState.SetCardLocked(true);
                cardView.CardState.SetUpgradedStatus(true);
            }
            else
            {
                _abilityAttributeData = cardView.CardData.AttributeData as ActiveAbilityData;
                _currentAbilityLevel = cardView.CardState.CurrentLevel;

                if (TryGetAbility(_abilityAttributeData as ActiveAbilityData, out Ability ability))
                    ability.Upgrade(ability.AbilityAttribute,
                        _currentAbilityLevel, _abilityDuration,
                        _abilityDamage,
                        _abilityCooldownReduction);
                else
                    MessageBroker.Default.Publish(new M_AbilityTake(
                        (_abilityAttributeData as ActiveAbilityData),
                       cardView.CardState.CurrentLevel));

                cardView.CardState.AddCurrentLevel();
                cardView.CardState.AddWeight();
            }
        }

        private void CreateClassAbility(ClassAbilityData abilityData)
        {
            ClassAbilityState classAbilityState =
                _persistentDataService.PlayerProgress.ClassAbilityService.GetClassAbilityStateById(abilityData.Id);
            MessageBroker.Default.Publish(new M_ClassAbilityTake(
                abilityData,
                classAbilityState.CurrentLevel));
        }

        private void OnAbilityUsed(Ability ability)
        {
            MessageBroker.Default.Publish(new M_AbilityUse(ability));
            _audioPlayer.PlayCharacterAudio(ability.AudioClip);
        }

        private void OnAbilityEnded(Ability ability)
        {
            MessageBroker.Default.Publish(new M_AbilityEnd(ability));
        }

        private void DestroyAbilities()
        {
            foreach (Ability ability in _abilities)
            {
                ability.AbilityUsed -= OnAbilityUsed;
                ability.AbilityEnded -= OnAbilityEnded;
                ability.Dispose();
            }

            foreach (var ability in _classAbilities)
            {
                ability.AbilityUsed -= OnAbilityUsed;
                ability.AbilityEnded -= OnAbilityEnded;
                ability.Dispose();
            }
        }

        private bool TrySetLegendaryAbility(TypeUpgradeAbility typeUpgrade, out Ability newability)
        {
            bool isFind = false;
            newability = null;

            if (_passiveAbilityViews.Count > 0)
            {
                if (_abilities.Count > 0)
                {
                    foreach (Ability ability in _abilities)
                    {
                        foreach (PassiveAbilityView passiveAbilityView in _passiveAbilityViews)
                        {
                            if (ability.TypeMagic == passiveAbilityView.TypeMagic)
                            {
                                if (ability.CurrentLevel == ability.MaxLevel - _shiftIndex)
                                {
                                    if (ability.TypeUpgradeMagic == typeUpgrade)
                                    {
                                        newability = ability;
                                        isFind = true;
                                        return isFind;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return isFind;
        }

        private bool TryGetAbility(ActiveAbilityData abilityAttributeData, out Ability oldAbility)
        {
            bool isFind = false;
            oldAbility = null;

            if (_abilities.Count > 0)
            {
                foreach (Ability ability in _abilities)
                {
                    if (ability.TypeAbility == abilityAttributeData.TypeAbility)
                    {
                        if (ability.TypeUpgradeMagic == abilityAttributeData.UpgradeType)
                        {
                            if (abilityAttributeData as AttackAbilityData != null)
                            {
                                if (ability.TypeAttackAbility ==
                                    (abilityAttributeData as AttackAbilityData).TypeAttackAbility)
                                {
                                    oldAbility = ability;
                                    isFind = true;
                                }
                            }
                            else
                            {
                                oldAbility = ability;
                                isFind = true;
                            }
                        }
                    }
                }
            }

            return isFind;
        }

        private void CreateLegendaryAbility(Ability ability, ActiveAbilityData abilityAttributeData)
        {
            foreach (Ability existingAbility in _legendaryAbilities)
            {
                if (existingAbility.TypeUpgradeMagic == ability.TypeUpgradeMagic)
                    return;
            }

            ability.Dispose();
            MessageBroker.Default.Publish(new M_LegendaryAbilityTake(
                (abilityAttributeData as AttackAbilityData).LegendaryAbilityData));
        }
    }
}