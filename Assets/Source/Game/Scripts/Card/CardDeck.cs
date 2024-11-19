using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class CardDeck : MonoBehaviour
    {
        [SerializeField] private PlayerStats _playerStats;

        private List<CardData> _cardDataAbility = new ();

        public List<CardData> CardDataAbility => _cardDataAbility;

        public void TakeCard(CardView cardView)
        {
            if (cardView.CardData.TypeCardParameter == TypeCardParameter.Ability)
            {
                _playerStats.SetNewAbility(cardView);

                if (_cardDataAbility.Count > 0)
                    AddCardAbilityData(cardView);
                else
                    _cardDataAbility.Add(cardView.CardData);
            }
            else if (cardView.CardData.TypeCardParameter == TypeCardParameter.RerollPoints)
            {
                _playerStats.UpdateRerollPoints(cardView);
            }
            else
            {
                _playerStats.UpdatePlayerStats(cardView);
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