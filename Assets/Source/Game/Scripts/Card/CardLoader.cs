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

        private CardDeck _deck;
        private List<CardData> _mainCardsPool = new ();

        public event Action CardPoolCreated;

        public List<CardData> MainCardsPool => _mainCardsPool;

        public void Initialize(CardDeck cardDeck)
        {
            _deck = cardDeck;

            foreach (var data in _cardData)
            {
                _deck.InitState(data);
            }
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

                int controlWeight = _deck.GetMinWeightCards();
                int currentCountCards = _deck.GetCountUnlockedCards();

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

                _deck.ResetCardState();
                CardPoolCreated?.Invoke();
            }
        }

        private CardData AddCard(List<CardData> cardsData, int controlWeight)
        {
            int currentWeightCards = 0;

            foreach (var card in cardsData)
            {
                CardState cardState = _deck.GetCardStateByData(card);

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
                CardState cardState = _deck.GetCardStateByData(card);

                if (card.Id != cardState.Id) continue;

                switch (card.TypeCardParameter)
                {
                    case TypeCardParameter.Ability:
                        ProcessAbilityCard(card, cardState, cards);
                        break;
                    case TypeCardParameter.PassiveAbility:
                        ProcessPassiveCard(card, cardState);
                        break;
                    case TypeCardParameter.LegendaryAbility:
                        ProcessLegendaryCard(card, cardState);
                        break;
                }
            }
        }

        private void ProcessPassiveCard(CardData card, CardState cardState)
        {
            if (cardState.CurrentLevel >= card.AttributeData.Parameters.Count)
            {
                cardState.SetCardLocked(true);
            }
        }

        private void ProcessAbilityCard(CardData card, CardState cardState, List<CardData> allCards)
        {
            bool canTakeCard = _deck.CanTakeAbilityCard(card.Id);

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
                if (TryFindPassivCard(allCards, activeAbility.MagicType))
                {
                    if (FindLegendaryCard(allCards, activeAbility.UpgradeType, out CardData legendaryCard))
                    {
                        _deck.GetCardStateByData(legendaryCard).SetCardLocked(false);
                    }
                }
            }
        }

        private void ProcessLegendaryCard(CardData card, CardState cardState)
        {
            if (cardState.CurrentLevel >= card.AttributeData.Parameters.Count)
            {
                cardState.SetCardLocked(true);
                cardState.SetUpgradedStatus(true);
            }
        }


        private bool FindLegendaryCard(List<CardData> cards, TypeUpgradeAbility typeMagic, out CardData legendaryCard)
        {
            legendaryCard = null;
            CardState cardState;

            foreach (var card in cards)
            {
                cardState = _deck.GetCardStateByData(card);

                if (card.AttributeData as LegendaryAbilityData)
                {
                    if (cardState.IsLocked)
                    {
                        if (typeMagic == (card.AttributeData as LegendaryAbilityData).UpgradeType)
                        {
                            if (cardState.CurrentLevel <= card.AttributeData.Parameters.Count)
                            {
                                if (cardState.IsCardUpgraded == false)
                                    legendaryCard = card;
                            }
                        }
                    }
                }
            }

            return legendaryCard != null;
        }

        private bool TryFindPassivCard(List<CardData> cards, TypeMagic typeMagic)
        {
            CardData passivCard = null;

            foreach (var data in cards)
            {
                if (data.AttributeData as PassiveAttributeData)
                {
                    if ((data.AttributeData as PassiveAttributeData).MagicType == typeMagic)
                    {
                        if (_deck.GetCardStateByData(data).IsCardUpgraded == true)
                        {
                            passivCard = data;
                        }
                    }
                }
            }

            return passivCard != null;
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