using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class CardLoader : MonoBehaviour
    {
        private readonly System.Random _rnd = new ();
        private readonly int _minValue = 0;
        private readonly int _shiftIndex = 1;
        private readonly int _maxCardsPool = 3;

        [SerializeField] private List<CardState> _cardState;
        [SerializeField] private List<CardData> _cardData;
        [SerializeField] private CardDeck _cardDeck;

        private List<CardData> _mainCardsPool = new ();
        private bool _isExtraAbilityLocked = false;

        public event Action CardPoolCreated;

        public List<CardData> MainCardsPool => _mainCardsPool;

        public void CreateCardPool()
        {
            if (_cardDeck.CardDataAbility.Count == _maxCardsPool)
                if(_isExtraAbilityLocked == false)
                    LockAbilityCards();

            if (_mainCardsPool.Count > _minValue)
                _mainCardsPool.Clear();

            Shuffle(_cardData);
            LockUsedCards(_cardData);
            GenerateMainPool(_cardData);
        }

        public CardState GetCardStateById(int id)
        {
            foreach (var cardState in _cardState)
            {
                if (id == cardState.Id)
                    return cardState;
            }

            return null;
        }

        private int GetMinWeightCards(List<CardState> cards)
        {
            int minWeight = 0;

            foreach (var card in cards)
            {
                if (card.IsLocked == false)
                    minWeight += card.Weight;
            }

            return _rnd.Next(_minValue, minWeight);
        }

        private int GetCountUnlockedCards(List<CardState> cards)
        {
            int countCards = 0;

            foreach (var card in cards)
            {
                if (card.IsLocked == false)
                    countCards++;
            }

            return countCards;
        }

        private void GenerateMainPool(List<CardData> cardsData)
        {
            if (cardsData.Count > _minValue)
            {
                int maxCardsPool;
                int currentWeightCards = 0;
                int controlWeight = GetMinWeightCards(_cardState);
                int currentCountCards = GetCountUnlockedCards(_cardState);

                if (currentCountCards < _maxCardsPool)
                    maxCardsPool = currentCountCards;
                else
                    maxCardsPool = _maxCardsPool;

                for (int index = 0; index < maxCardsPool; index++)
                {
                    CardData newCard = AddCard(cardsData, currentWeightCards, controlWeight);

                    if (newCard != null)
                        _mainCardsPool.Add(newCard);
                }

                ResetCardState();
                CardPoolCreated?.Invoke();
            }
        }

        private CardData AddCard(List<CardData> cardsData, int currentWeightCards, int controlWeight)
        {
            foreach (var card in cardsData)
            {
                foreach (var cardState in _cardState)
                {
                    if (card.Id == cardState.Id)
                    {
                        if (cardState.IsLocked == false)
                        {
                            if (cardState.IsTaked == false)
                            {
                                currentWeightCards += cardState.Weight;

                                if (currentWeightCards >= controlWeight)
                                {
                                    cardState.IsTaked = true;
                                    return card;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private void ResetCardState()
        {
            foreach (var card in _cardState)
            {
                card.IsTaked = false;
            }
        }
        //private CardData AddCard(List<CardData> cardsData, int currentWeightCards, int controlWeight)
        //{
        //    foreach (var card in cardsData)
        //    {
        //        foreach (var cardState in _cardState)
        //        {
        //            if (card.Id == cardState.Id)
        //            {
        //                if (TryFindSameCard(cardState) == false)
        //                {
        //                    if (cardState.IsLocked == false)
        //                    {
        //                        currentWeightCards += cardState.Weight;

        //                        if (currentWeightCards >= controlWeight)
        //                            return card;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

        private bool TryFindSameCard(CardState cardState)
        {
            if (_mainCardsPool.Count > _minValue)
            {
                foreach (CardData card in _mainCardsPool)
                {
                    if (card.Id == cardState.Id)
                        return true;
                }
            }

            return false;
        }

        private void LockAbilityCards()
        {
            List<CardData> tempDatas = new ();
            _cardData.ForEach(card => tempDatas.Add(card));

            for (int index = 0; index < _cardDeck.CardDataAbility.Count; index++)
            {
                if (tempDatas.Contains(_cardDeck.CardDataAbility[index]))
                    tempDatas.Remove(_cardDeck.CardDataAbility[index]);
            }

            foreach (var tempData in tempDatas)
            {
                foreach (var cardState in _cardState)
                {
                    if (tempData.Id == cardState.Id)
                        if (tempData.TypeCardParameter == TypeCardParameter.Ability)
                            cardState.IsLocked = true;
                }
            }

            _isExtraAbilityLocked = true;
        }

        private void LockUsedCards(List<CardData> cards)
        {
            foreach (var card in cards)
            {
                foreach (var cardState in _cardState)
                {
                    if (card.Id == cardState.Id)
                    {
                        if (card.AttributeData.CardParameters.Count <= cardState.CurrentLevel)
                            cardState.IsLocked = true;
                    }
                }
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