using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class CardKnowledgebaseView : KnowledgeView
    {
        [SerializeField] private Image _cardIcon;
        [SerializeField] private Image _cardTear;
        [SerializeField] private LeanLocalizedText _cardName;
        [SerializeField] private LeanLocalizedText _cardDescription;
        [Space(20)]
        [SerializeField] private Transform _linkCardContainer;
        [SerializeField] private LinkCardView _linkCardView;
        [SerializeField] private BonusCardView _bonusCardView;
        [Space(20)]
        [SerializeField] private KnowledgeCardParameterView _parameterView;
        [SerializeField] private Transform _parameterContainer;

        public void Initialize(CardData cardData)
        {
            _cardTear.color = new Color(
                    cardData.TypeCardColor[(int)cardData.TypeCardParameter].r,
                    cardData.TypeCardColor[(int)cardData.TypeCardParameter].g,
                    cardData.TypeCardColor[(int)cardData.TypeCardParameter].b);

            _cardIcon.sprite = cardData.AttributeData.Icon;
            _cardName.TranslationName = cardData.AttributeData.Name;
            _cardDescription.TranslationName = cardData.AttributeData.Description;

            CreateSupportCardsView(cardData);
            CreateParameterView(cardData);
        }

        private void CreateSupportCardsView(CardData cardData)
        {
            if (cardData.SupportiveData.Count <= 0)
            {
                Instantiate(_bonusCardView, _linkCardContainer);
                return;
            }

            CreateSupportCardViews(cardData);
            TryCreateLegendaryLinkCard(cardData);
        }

        private void CreateSupportCardViews(CardData cardData)
        {
            foreach (var supportData in cardData.SupportiveData)
            {
                LinkCardView cardView = Instantiate(_linkCardView, _linkCardContainer);
                cardView.Initialize(supportData.Icon, supportData.Name);
            }
        }

        private void TryCreateLegendaryLinkCard(CardData cardData)
        {
            if (cardData.AttributeData is LegendaryAbilityData legendaryData &&
                cardData.TypeCardParameter != TypeCardParameter.LegendaryAbility)
            {
                LinkCardView cardView = Instantiate(_linkCardView, _linkCardContainer);
                cardView.Initialize(legendaryData.Icon, legendaryData.Name);
            }
        }

        private void CreateParameterView(CardData cardData)
        {
            if (cardData.AttributeData == null)
                return;

            List<Parameters> parameters = cardData.AttributeData switch
            {
                LegendaryAbilityData legendaryData => legendaryData.Parameters,
                _ => cardData.AttributeData.Parameters
            };

            for (int i = 0; i < parameters.Count; i++)
            {
                KnowledgeCardParameterView view = Instantiate(_parameterView, _parameterContainer);
                view.Initialize(cardData, i);
            }
        }
    }
}