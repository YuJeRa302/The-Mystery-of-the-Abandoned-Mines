using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class CardDeck
    {
        private readonly System.Random _rnd = new ();
        private readonly int _defaultStateLevel = 0;
        private readonly int _defaultStateWeight = 1;
        private readonly int _defaultLevelCardCount = 3;
        private readonly int _contrackLevelCardCount = 2;

        private List<CardData> _cardDataAbility = new ();
        private List<CardData> _activeCardAbility = new ();
        private List<CardState> _cardState = new ();
        private int _currentMaxCardCount;

        public event Action<CardView> SetNewAbility;
        public event Action<CardView> RerollPointsUpdated;
        public event Action<CardView> PlayerStatsUpdated;
        public event Action<CardView> TakedPassivAbility;

        public CardDeck(bool isCpntractLevel)
        {
            if (isCpntractLevel)
                _currentMaxCardCount = _contrackLevelCardCount;
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
                case TypeCardParameter.PassivAbllity:
                    TakePassiveAbility(cardView);
                    break;
                case TypeCardParameter.LegendariAbility:
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
                card.IsTaked = false;
            }
        }

        public void UpdateDeck()
        {
            foreach (var card in _cardState)
            {
                card.Weight++;
            }
        }

        public CardState InitState(CardData cardData)
        {
            CardState cardState = new ();
            cardState.Id = cardData.Id;
            cardState.CurrentLevel = _defaultStateLevel;

            if (cardData.TypeCardParameter == TypeCardParameter.LegendariAbility)
                cardState.IsLocked = true;
            else
                cardState.IsLocked = false;

            cardState.Weight = _defaultStateWeight;
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

            TakedPassivAbility?.Invoke(cardView);
        }

        private void AddCardAbilityData(CardView cardView, List<CardData> repository)
        {
            if (repository.Contains(cardView.CardData))
                return;

            repository.Add(cardView.CardData);
        }
    }
}