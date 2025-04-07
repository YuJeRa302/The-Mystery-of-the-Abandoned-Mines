using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class CardDeck
    {
        private readonly System.Random _rnd = new ();
        private readonly int _defaultStateLevel = 0;
        private readonly int _defaultStateWeight = 1;
        private readonly int _minValue = 0;

        private List<CardData> _cardDataAbility = new ();
        private List<CardData> _activeCardAbility = new ();
        private List<CardState> _cardState = new ();

        public event Action<CardView> SetNewAbility;
        public event Action<CardView> RerollPointsUpdated;
        public event Action<CardView> PlayerStatsUpdated;
        public event Action<CardView> TakedPassivAbility;

        public CardDeck() { }

        public List<CardData> CardDataAbility => _cardDataAbility;
        public List<CardState> CardStates => _cardState;

        public void TakeCard(CardView cardView)
        {
            if (cardView.CardData.TypeCardParameter == TypeCardParameter.Ability || cardView.CardData.TypeCardParameter == TypeCardParameter.LegendariAbility 
                || cardView.CardData.TypeCardParameter == TypeCardParameter.PassivAbllity)
            {
                if(cardView.CardData.TypeCardParameter != TypeCardParameter.PassivAbllity)
                    SetNewAbility?.Invoke(cardView);

                if (_cardDataAbility.Count > 0)
                    AddCardAbilityData(cardView, _cardDataAbility);
                else
                    _cardDataAbility.Add(cardView.CardData);

                if (cardView.CardData.AttributeData as AttackAbilityData)
                {
                    if (_activeCardAbility.Count > 0)
                    {
                        AddCardAbilityData(cardView, _activeCardAbility);
                    }
                    else
                    {
                        _activeCardAbility.Add(cardView.CardData);
                    }
                }
                else if (cardView.CardData.AttributeData as PassiveAttributeData)
                {
                    TakedPassivAbility?.Invoke(cardView);
                }
                cardView.CardState.CurrentLevel++;
                cardView.CardState.Weight++;
            }
            else if (cardView.CardData.TypeCardParameter == TypeCardParameter.RerollPoints)
            {
                RerollPointsUpdated?.Invoke(cardView);
            }
            else
            {
                PlayerStatsUpdated?.Invoke(cardView);
            }
            //cardView.CardState.Weight++;
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
            if (_activeCardAbility.Count < 3)
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

        private void AddCardAbilityData(CardView cardView, List<CardData> repository)
        {
            if (repository.Contains(cardView.CardData))
                return;

            repository.Add(cardView.CardData);
        }
    }
}