using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Card
{
    public class CardLoader : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();
        private readonly int _minValue = 0;
        private readonly int _shiftIndex = 1;
        private readonly int _maxCardsPool = 3;

        [SerializeField] private List<CardData> _cardData;
        [SerializeField] private List<CardData> _defaultCardData;

        private Player _player;
        private List<CardData> _mainCardsPool = new ();

        public event Action CardPoolCreated;

        public List<CardData> MainCardsPool => _mainCardsPool;

        public void Initialize(Player player)
        {
            _player = player;
            _player.InitStateCardDeck(_cardData);
        }

        public void CreateCardPool()
        {
            if (_mainCardsPool.Count > _minValue)
                _mainCardsPool.Clear();

            Shuffle(_cardData);
            LockUsedCards(_cardData);
            GenerateMainPool(_cardData);
        }

        private void GenerateMainPool(List<CardData> cardsData)
        {
            if (cardsData.Count > _minValue)
            {
                int maxCardsPool;

                int controlWeight = _player.GetMinWeightCards();
                int currentCountCards = _player.GetCountUnlockedCards();

                if (currentCountCards < _maxCardsPool)
                    maxCardsPool = currentCountCards;
                else
                    maxCardsPool = _maxCardsPool;

                for (int index = 0; index < maxCardsPool; index++)
                {
                    CardData newCard = AddCard(cardsData, controlWeight);

                    if (newCard != null)
                        _mainCardsPool.Add(newCard);
                    else
                        _mainCardsPool.Add(AddDefaultCard());
                }

                _player.ResetCardState();
                CardPoolCreated?.Invoke();
            }
        }

        private CardData AddCard(List<CardData> cardsData, int controlWeight)
        {
            int currentWeightCards = 0;

            foreach (var card in cardsData)
            {
                CardState cardState = _player.GetCardStateByData(card);

                if (cardState.IsLocked == false)
                {
                    if (cardState.IsTaked == false)
                    {
                        currentWeightCards += cardState.Weight;

                        if (currentWeightCards >= controlWeight)
                        {
                            cardState.SetCardLocked(true);
                            currentWeightCards = 0;
                            return card;
                        }
                    }
                }
            }

            return null;
        }

        private CardData AddDefaultCard()
        {
            return _defaultCardData[_rnd.Next(_minValue, _defaultCardData.Count)];
        }

        private void LockUsedCards(List<CardData> cards)
        {
            foreach (var card in cards)
            {
                CardState cardState = _player.GetCardStateByData(card);

                if (card.Id != cardState.Id) continue;

                switch (card.TypeCardParameter)
                {
                    case TypeCardParameter.Ability:
                        ProcessAbilityCard(card, cardState, cards);
                        break;

                    case TypeCardParameter.LegendaryAbility:
                        ProcessLegendaryCard(card, cardState);
                        break;
                }
            }
        }

        private bool FindLegendaryCard(
            List<CardData> cards,
            TypeUpgradeAbility upgradeType,
            out CardData legendaryCard)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];

                if (card.AttributeData as LegendaryAbilityData)
                {
                    var cardState = _player.GetCardStateByData(card);

                    if (cardState.IsLocked)
                    {
                        if ((card.AttributeData as LegendaryAbilityData).UpgradeType == upgradeType)
                        {
                            if (cardState.CurrentLevel <=
                                (card.AttributeData as LegendaryAbilityData).Parameters.Count)
                            {
                                if (!cardState.IsCardUpgraded)
                                {
                                    legendaryCard = card;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            legendaryCard = null;
            return false;
        }

        private bool TryFindPassiveCard(List<CardData> cards, TypeMagic magicType)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];

                if (card.AttributeData is PassiveAttributeData passiveData &&
                    passiveData.MagicType == magicType &&
                    _player.GetCardStateByData(card).IsLocked)
                {
                    return true;
                }
            }

            return false;
        }

        private void ProcessAbilityCard(CardData card, CardState cardState, List<CardData> allCards)
        {
            bool isPassive = card.AttributeData is PassiveAttributeData;
            bool canTakeCard = _player.TryTakeAbilityCard(card.Id) || isPassive;

            if (!canTakeCard)
            {
                cardState.SetCardLocked(true);
                return;
            }

            if (card.AttributeData.Parameters.Count > cardState.CurrentLevel)
                return;

            cardState.SetCardLocked(true);

            if (card.AttributeData is ActiveAbilityData activeAbility)
            {
                if (TryFindPassiveCard(allCards, activeAbility.MagicType))
                {
                    if (FindLegendaryCard(allCards, activeAbility.UpgradeType, out CardData legendaryCard))
                    {
                        _player.GetCardStateByData(legendaryCard).SetCardLocked(false);
                    }
                }
            }
        }

        private void ProcessLegendaryCard(CardData card, CardState cardState)
        {
            if (card.AttributeData is LegendaryAbilityData legendaryData &&
                legendaryData.Parameters.Count <= cardState.CurrentLevel)
            {
                cardState.SetCardLocked(true);
                cardState.SetUpgradedStatus(true);
            }
        }

        private void Shuffle(List<CardData> cards)
        {
            if (cards.Count > _minValue)
            {
                int numberMax = cards.Count - _shiftIndex;

                for (int index = 0; index < cards.Count; index++)
                {
                    int randomNumber = _rnd.Next(_minValue, numberMax);
                    CardData tempCard = cards[randomNumber];
                    cards[randomNumber] = cards[index];
                    cards[index] = tempCard;
                }
            }
        }
    }
}