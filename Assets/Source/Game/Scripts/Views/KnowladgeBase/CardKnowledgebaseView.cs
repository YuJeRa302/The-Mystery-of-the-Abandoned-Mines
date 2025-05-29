using Assets.Source.Game.Scripts;
using Lean.Localization;
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

        if (cardData.AttributeData != null)
        {
            _cardIcon.sprite = cardData.AttributeData.Icon;
            _cardName.TranslationName = cardData.AttributeData.NameCard;
            _cardDiscription.TranslationName = cardData.AttributeData.Description;
        }
        else
        {
            //LegendaryAbilityData legendaryAbilityData = (cardData.AttributeData as AbilityAttributeData).LegendaryAbilityData;
            _cardIcon.sprite = cardData.LegendaryAbilityData.Icon;
            _cardName.TranslationName = cardData.LegendaryAbilityData.Name;
            _cardDiscription.TranslationName = cardData.LegendaryAbilityData.Description;
        }

        CreateSupportivParametrField(cardData);
        CreateParametrView(cardData);
    }

    private void CreateSupportivParametrField(CardData cardData)
    {
        if (cardData.SuppurtivData.Count <= 0)
        {
            Instantiate(_bonusCardView, _linkCardConteiner);
            return;
        }

        for (int i = 0; i < cardData.SuppurtivData.Count; i++)
        {
            LinkCardView cardView = Instantiate(_linkCardView, _linkCardConteiner);
            cardView.Initialize(cardData.SuppurtivData[i].Icon, cardData.SuppurtivData[i].NameCard);
        }

        //LegendaryAbilityData legendaryAbilityData = (cardData.AttributeData as AbilityAttributeData).LegendaryAbilityData;

        if (cardData.LegendaryAbilityData != null)
        {
            if(cardData.TypeCardParameter != TypeCardParameter.LegendariAbility)
            {
                LinkCardView cardView = Instantiate(_linkCardView, _linkCardConteiner);
                // cardView.Initialize(legendaryAbilityData.Icon, legendaryAbilityData.Name);
                cardView.Initialize(cardData.LegendaryAbilityData.Icon, cardData.LegendaryAbilityData.Name);
            }
        }
    }

    private void CreateParametrView(CardData cardData)
    {
        if (cardData.AttributeData != null)
        {
            for (int i = 0; i < cardData.AttributeData.CardParameters.Count; i++)
            {
                KnowleadgeCardParameterView view = Instantiate(_parameterView, _parametrConteiner);
                view.Initialize(cardData, i);
            }
        }
        else
        {
            //LegendaryAbilityData legendaryAbilityData = (cardData.AttributeData as AbilityAttributeData).LegendaryAbilityData;

            for (int i = 0; i < cardData.LegendaryAbilityData.LegendaryAbilityParameters.Count; i++)
            {
                KnowleadgeCardParameterView view = Instantiate(_parameterView, _parametrConteiner);
                view.Initialize(cardData, i);
            }
        }
    }
}