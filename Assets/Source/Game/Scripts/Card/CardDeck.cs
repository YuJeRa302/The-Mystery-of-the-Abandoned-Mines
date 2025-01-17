using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class CardDeck
    {
        private List<CardData> _cardDataAbility = new ();

        public event Action<CardView> SetNewAbility;
        public event Action<CardView> RerollPointsUpdated;
        public event Action<CardView> PlayerStatsUpdated;

        public CardDeck() { }

        public List<CardData> CardDataAbility => _cardDataAbility;

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
        }

        private void AddCardAbilityData(CardView cardView)
        {
            if (_cardDataAbility.Contains(cardView.CardData))
                return;

            _cardDataAbility.Add(cardView.CardData);
        }
    }
}