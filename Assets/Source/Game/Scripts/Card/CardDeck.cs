using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UniRx;

namespace Assets.Source.Game.Scripts.Card
{
    public class CardDeck
    {
        public static readonly IMessageBroker MessageBroker = new MessageBroker();

        private readonly System.Random _rnd = new ();
        private readonly int _defaultStateLevel = 0;
        private readonly int _defaultLevelCardCount = 3;
        private readonly int _contractLevelCardCount = 2;
        private readonly int _weightControl = 100;
        private readonly List<ITakeCardStrategy> _takeCardStrategies;

        private List<CardData> _cardDataAbility = new ();
        private List<CardData> _activeCardAbility = new ();
        private List<CardState> _cardState = new ();
        private int _currentMaxCardCount;
        private bool _isReserWeight = false;

        public CardDeck(bool isContractLevel, List<ITakeCardStrategy> takeCardStrategies)
        {
            if (isContractLevel)
                _currentMaxCardCount = _contractLevelCardCount;
            else
                _currentMaxCardCount = _defaultLevelCardCount;

            _takeCardStrategies = takeCardStrategies;
            _takeCardStrategies.ForEach(s => s.Construct(_cardDataAbility, _activeCardAbility));
        }

        public void TakeCard(CardView cardView)
        {
            _takeCardStrategies.ForEach(s => s.TakeCard(cardView));
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
                    card.ResetWeight();
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
            CardState cardState = new(
                cardData.Id,
                cardData.TypeCardParameter == TypeCardParameter.LegendaryAbility, 
                _defaultStateLevel,
                cardData.TypeCardParameter == TypeCardParameter.LegendaryAbility);

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
    }
}