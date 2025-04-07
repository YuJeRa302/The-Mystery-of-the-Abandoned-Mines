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

        [SerializeField] private List<CardData> _cardData;
        [SerializeField] private List<CardData> _defaultCardData;

        private Player _player;
        private List<CardData> _mainCardsPool = new ();

        public event Action CardPoolCreated;

        public List<CardData> MainCardsPool => _mainCardsPool;

        public void Initialize(Player player)
        {
            _player = player;
            _player.InitStateCardDeck(_cardData);
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

                int controlWeight = _player.GetMinWeightCards();
                int currentCountCards = _player.GetCountUnlockedCards();

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

                _player.ResetCardState();
                CardPoolCreated?.Invoke();
            }
        }

        private CardData AddCard(List<CardData> cardsData, int controlWeight)
        {
            int currentWeightCards = 0;

            foreach (var card in cardsData)
            {
                CardState cardState = _player.GetCardStateByData(card);
                
                if (cardState.IsLocked == false)
                {
                    if (cardState.IsTaked == false)
                    {
                        currentWeightCards += cardState.Weight;

                        if (currentWeightCards >= controlWeight)
                        {
                            cardState.IsTaked = true;
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
                CardState cardState = _player.GetCardStateByData(card);

                if (card.Id == cardState.Id)
                {
                    if (card.TypeCardParameter == TypeCardParameter.Ability)
                    {
                        if(_player.TryTakeAbilityCard(card.Id) || card.AttributeData as PassiveAttributeData)
                        {
                            if (card.AttributeData.CardParameters.Count <= cardState.CurrentLevel)
                            {
                                cardState.IsLocked = true;

                                if (card.AttributeData as AbilityAttributeData)
                                {
                                    if (TryFindPassivCard(cards, (card.AttributeData as AbilityAttributeData).TypeMagic))
                                    {
                                        if (FindLegendaryCard(cards, (card.AttributeData as AbilityAttributeData).TypeUpgradeMagic, out CardData legendaryCard))
                                            _player.GetCardStateByData(legendaryCard).IsLocked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cardState.IsLocked = true;
                        }
                    }
                    else if (card.TypeCardParameter == TypeCardParameter.LegendariAbility)
                    {
                        if (card.LegendaryAbilityData.LegendaryAbilityParameters.Count <= cardState.CurrentLevel)
                        {
                            cardState.IsLocked = true;
                            cardState.IsCardUpgraded = true;
                        }
                    }
                }
            }
        }

        private bool FindLegendaryCard(List<CardData> cards, TypeUpgradeAbility typeMagic, out CardData legendaryCard)
        {
            legendaryCard = null;

            foreach (var card in cards)
            {
                if (card.LegendaryAbilityData != null)
                {
                    if (_player.GetCardStateByData(card).IsLocked)
                    {
                        if (typeMagic == card.LegendaryAbilityData.TypeUpgradeMagic)
                        {
                            if (_player.GetCardStateByData(card).CurrentLevel <= card.LegendaryAbilityData.LegendaryAbilityParameters.Count)
                            {
                                if (_player.GetCardStateByData(card).IsCardUpgraded == false)
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
                    if ((data.AttributeData as PassiveAttributeData).TypeMagic == typeMagic)
                    {
                        if (_player.GetCardStateByData(data).IsLocked == true)
                        {
                            passivCard = data;
                        }
                    }
                }
            }

            return passivCard != null;
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