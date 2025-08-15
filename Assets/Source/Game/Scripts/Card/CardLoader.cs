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
        private Dictionary<TypeCardParameter, IProcessCard> _cardHandlers;

        public event Action CardPoolCreated;

        public List<CardData> MainCardsPool => _mainCardsPool;

        public void Initialize(CardDeck cardDeck)
        {
            _deck = cardDeck;

            foreach (var data in _cardData)
            {
                _deck.InitState(data);
            }

            _cardHandlers = new Dictionary<TypeCardParameter, IProcessCard>()
            {
                {TypeCardParameter.Ability, new ProcessAbilityCard(_mainCardsPool, _deck)},
                {TypeCardParameter.PassiveAbility, new ProcessPassiveCard()},
                { TypeCardParameter.LegendaryAbility, new ProcessLegendaryCard()}
            };
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

                if (card.Id != cardState.Id)
                    continue;

                if (_cardHandlers.TryGetValue(card.TypeCardParameter, out IProcessCard process))
                    process.ProcessCard(card, cardState);
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