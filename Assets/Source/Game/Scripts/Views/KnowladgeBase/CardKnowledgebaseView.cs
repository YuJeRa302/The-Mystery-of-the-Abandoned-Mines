using Assets.Source.Game.Scripts;
using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardKnowledgebaseView : KnowladgeView
{
    [SerializeField] private Image _cardIcon;
    [SerializeField] private Image _cardTear;
    [SerializeField] private LeanLocalizedText _cardName;
    [SerializeField] private LeanLocalizedText _cardDiscription;
    [Space(20)]
    [SerializeField] private Transform _linkCardConteiner;
    [SerializeField] private LinkCardView _linkCardView;
    [SerializeField] private BonusCardView _bonusCardView;
    [Space(20)]
    [SerializeField] private KnowleadgeCardParameterView _parameterView;
    [SerializeField] private Transform _parametrConteiner;

    public void Initialize(CardData cardData)
    {
        _cardTear.color = new Color(
                cardData.TypeCardColor[(int)cardData.TypeCardParameter].r,
                cardData.TypeCardColor[(int)cardData.TypeCardParameter].g,
                cardData.TypeCardColor[(int)cardData.TypeCardParameter].b);

        _cardIcon.sprite = cardData.AttributeData.Icon;
        _cardName.TranslationName = cardData.AttributeData.Name;
        _cardDiscription.TranslationName = cardData.AttributeData.Description;

        CreateSupportCardsView(cardData);
        CreateParametrView(cardData);
    }

    private void CreateSupportCardsView(CardData cardData)
    {
        if (cardData.SuppurtivData.Count <= 0)
        {
            Instantiate(_bonusCardView, _linkCardConteiner);
            return;
        }

        CreateSupportCardViews(cardData);
        TryCreateLegendaryLinkCard(cardData);
    }

    private void CreateSupportCardViews(CardData cardData)
    {
        foreach (var supportData in cardData.SuppurtivData)
        {
            LinkCardView cardView = Instantiate(_linkCardView, _linkCardConteiner);
            cardView.Initialize(supportData.Icon, supportData.Name);
        }
    }

    private void TryCreateLegendaryLinkCard(CardData cardData)
    {
        if (cardData.AttributeData is LegendaryAbilityData legendaryData &&
            cardData.TypeCardParameter != TypeCardParameter.LegendariAbility)
        {
            LinkCardView cardView = Instantiate(_linkCardView, _linkCardConteiner);
            cardView.Initialize(legendaryData.Icon, legendaryData.Name);
        }
    }

    private void CreateParametrView(CardData cardData)
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
            KnowleadgeCardParameterView view = Instantiate(_parameterView, _parametrConteiner);
            view.Initialize(cardData, i);
        }
    }
}