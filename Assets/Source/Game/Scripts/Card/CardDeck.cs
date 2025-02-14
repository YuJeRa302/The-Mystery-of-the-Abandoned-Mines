using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class CardDeck
    {
        private readonly Random _rnd = new ();
        private readonly int _defaultStateLevel = 0;
        private readonly int _defaultStateWeight = 1;
        private readonly int _minValue = 0;

        private List<CardData> _cardDataAbility = new ();
        private List<CardState> _cardState = new ();

        public event Action<CardView> SetNewAbility;
        public event Action<CardView> RerollPointsUpdated;
        public event Action<CardView> PlayerStatsUpdated;

        public CardDeck() { }

        public List<CardData> CardDataAbility => _cardDataAbility;
        public List<CardState> CardStates => _cardState;

        public void TakeCard(CardView cardView)
        {
            if (cardView.CardData.TypeCardParameter == TypeCardParameter.Ability)
            {
                SetNewAbility?.Invoke(cardView);

                if (_cardDataAbility.Count > 0)
                    AddCardAbilityData(cardView);
                else
                    _cardDataAbility.Add(cardView.CardData);
            }
            else if (cardView.CardData.TypeCardParameter == TypeCardParameter.RerollPoints)
            {
                RerollPointsUpdated?.Invoke(cardView);
            }
            else
            {
                PlayerStatsUpdated?.Invoke(cardView);
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
            int minWeight = 0;

            foreach (var card in _cardState)
            {
                if (card.IsLocked == false)
                    minWeight += card.Weight;
            }

            return _rnd.Next(_minValue, minWeight);
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

        private CardState InitState(CardData cardData)//паблик вызывать в сардодер при инициализации+проверка на легендарку
        {
            CardState cardState = new ();
            cardState.Id = cardData.Id;
            cardState.CurrentLevel = _defaultStateLevel;
            cardState.IsLocked = false;
            cardState.Weight = _defaultStateWeight;
            _cardState.Add(cardState);
            return cardState;
        }

        private void AddCardAbilityData(CardView cardView)
        {
            if (_cardDataAbility.Contains(cardView.CardData))
                return;

            _cardDataAbility.Add(cardView.CardData);
        }
    }
}