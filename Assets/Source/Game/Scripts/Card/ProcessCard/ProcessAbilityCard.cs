using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Card
{
    public class ProcessAbilityCard : IProcessCard
    {
        private CardLoader _cardLoader;
        private CardDeck _deck;

        public ProcessAbilityCard(CardLoader cardLoader, CardDeck cardDeck)
        {
            _cardLoader = cardLoader;
            _deck = cardDeck;
        }

        public void ProcessCard(CardData cardData, CardState cardState)
        {
            bool canTakeCard = _deck.CanTakeAbilityCard(cardData.Id);

            if (!canTakeCard)
            {
                cardState.SetCardLocked(true);
                return;
            }

            if (cardData.AttributeData.Parameters.Count > cardState.CurrentLevel)
                return;

            cardState.SetCardLocked(true);

            if (cardData.AttributeData as ActiveAbilityData)
            {
                if (TryFindPassivCard(_cardLoader.CardData, (cardData.AttributeData as ActiveAbilityData).MagicType))
                {
                    if (FindLegendaryCard(_cardLoader.CardData, (cardData.AttributeData as ActiveAbilityData).UpgradeType, out CardData legendaryCard))
                    {
                        _deck.GetCardStateByData(legendaryCard).SetCardLocked(false);
                    }
                }
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
    }
}