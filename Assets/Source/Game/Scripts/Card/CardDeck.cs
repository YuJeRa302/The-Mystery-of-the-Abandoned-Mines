using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Card
{
    public class CardDeck
    {
        private readonly System.Random _rnd = new();
        private readonly int _defaultStateLevel = 0;
        private readonly int _defaultLevelCardCount = 3;
        private readonly int _contractLevelCardCount = 2;
        private readonly int _weightControl = 100;

        private List<CardData> _cardDataAbility = new();
        private List<CardData> _activeCardAbility = new();
        private List<CardState> _cardState = new();
        private int _currentMaxCardCount;
        private bool _isReserWeight = false;

        public event Action<CardView> SetNewAbility;
        public event Action<CardView> RerollPointsUpdated;
        public event Action<CardView> PlayerStatsUpdated;
        public event Action<CardView> TakedPassiveAbility;

        public CardDeck(bool isContractLevel)
        {
            if (isContractLevel)
                _currentMaxCardCount = _contractLevelCardCount;
            else
                _currentMaxCardCount = _defaultLevelCardCount;
        }

        public void TakeCard(CardView cardView)
        {
            switch (cardView.CardData.TypeCardParameter)
            {
                case TypeCardParameter.RerollPoints:
                    RerollPointsUpdated?.Invoke(cardView);
                    break;
                case TypeCardParameter.Default:
                    PlayerStatsUpdated?.Invoke(cardView);
                    break;
                case TypeCardParameter.PassiveAbility:
                    TakePassiveAbility(cardView);
                    break;
                case TypeCardParameter.LegendaryAbility:
                    TakeAbility(cardView);
                    break;
                case TypeCardParameter.Ability:
                    TakeAbility(cardView);
                    break;
            }

            _cardState.Add(cardView.CardState);
        }

        public CardState GetCardStateByData(CardData cardData)
        {
            CardState cardState = null;

            foreach (var state in _cardState)
            {
                if (cardData.Id == state.Id)
                    cardState = state;
            }

            if (cardState == null)
                cardState = InitState(cardData);

            return cardState;
        }

        public int GetMinWeightCards()
        {
            int maxWeight = 0;
            int minWeight = 0;

            foreach (var card in _cardState)
            {
                if (card.IsLocked == false)
                {
                    maxWeight += card.Weight;

                    if (minWeight == 0)
                        minWeight = card.Weight;

                    if (minWeight > card.Weight)
                        minWeight = card.Weight;
                }
            }

            if (maxWeight > _weightControl)
                _isReserWeight = true;

            return _rnd.Next(minWeight, maxWeight);
        }

        public int GetCountUnlockedCards()
        {
            int countCards = 0;

            foreach (var card in _cardState)
            {
                if (card.IsLocked == false)
                    countCards++;
            }

            return countCards;
        }

        public void ResetCardState()
        {
            foreach (var card in _cardState)
            {
                if (card.IsLegendariCard == false)
                    card.SetCardLocked(false);

                if (_isReserWeight)
                {
                    card.ResetWeight();
                }
            }

            _isReserWeight = false;
        }

        public void UpdateDeck()
        {
            foreach (var card in _cardState)
            {
                card.AddWeight();
            }
        }

        public CardState InitState(CardData cardData)
        {
            bool isLocked;
            bool isLegendariCard;

            if (cardData.TypeCardParameter == TypeCardParameter.LegendaryAbility)
            {
                isLegendariCard = true;
                isLocked = true;
            }
            else
            {
                isLocked = false;
                isLegendariCard = false;
            }


            CardState cardState = new(cardData.Id, isLocked, _defaultStateLevel, isLegendariCard);
            _cardState.Add(cardState);
            return cardState;
        }

        public bool CanTakeAbilityCard(int id)
        {
            if (_activeCardAbility.Count < _currentMaxCardCount)
            {
                return true;
            }
            else
            {
                foreach (CardData cardData in _activeCardAbility)
                {
                    if (cardData.Id == id)
                        return true;
                }
            }

            return false;
        }

        private void TakeAbility(CardView cardView)
        {
            SetNewAbility?.Invoke(cardView);

            if (_cardDataAbility.Count > 0)
                AddCardAbilityData(cardView, _cardDataAbility);
            else
                _cardDataAbility.Add(cardView.CardData);

            if (cardView.CardData.AttributeData as AttackAbilityData)
            {
                if (_activeCardAbility.Count > 0)
                    AddCardAbilityData(cardView, _activeCardAbility);
                else
                    _activeCardAbility.Add(cardView.CardData);
            }
        }

        private void TakePassiveAbility(CardView cardView)
        {
            if (_cardDataAbility.Count > 0)
                AddCardAbilityData(cardView, _cardDataAbility);
            else
                _cardDataAbility.Add(cardView.CardData);

            TakedPassiveAbility?.Invoke(cardView);
        }

        private void AddCardAbilityData(CardView cardView, List<CardData> repository)
        {
            if (repository.Contains(cardView.CardData))
                return;

            repository.Add(cardView.CardData);
        }
    }
}