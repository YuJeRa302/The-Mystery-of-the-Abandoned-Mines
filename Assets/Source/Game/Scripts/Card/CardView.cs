using System;
using System.Collections.Generic;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image _cardIcon;
        [SerializeField] private Image _cardImage;
        [SerializeField] private Button _applyButton;
        [SerializeField] private LeanLocalizedText _cardName;
        [SerializeField] private LeanLocalizedText _description;
        [SerializeField] private Transform _cardParameterContainer;
        [SerializeField] private Transform _supportiParametrContainer;
        [SerializeField] private CardSupportivParametrsView _supportivParametrsView;

        private List<CardParameterView> _cardParametersViews = new ();
        private CardState _cardState;
        private CardData _cardData;

        public event Action<CardView> CardTaked;

        public CardData CardData => _cardData;
        public CardState CardState => _cardState;


        private void OnDestroy()
        {
            _applyButton.onClick.RemoveListener(TakeCard);
        }

        public void Initialize(CardState cardState, CardData cardData)
        {
            _cardData = cardData;
            _cardState = cardState;

            if (cardData.AttributeData != null)
            {
                _cardIcon.sprite = cardData.AttributeData.Icon;
                _cardName.TranslationName = cardData.AttributeData.NameCard;
                _description.TranslationName = cardData.AttributeData.Description;
            }
            else
            {
                LegendaryAbilityData legendaryAbilityData = (cardData.AttributeData as AbilityAttributeData).LegendaryAbilityData;
                _cardIcon.sprite = legendaryAbilityData.Icon;
                _cardName.TranslationName = legendaryAbilityData.Name;
                _description.TranslationName = legendaryAbilityData.Description;
            }

            _cardImage.color = new Color(
                cardData.TypeCardColor[(int)cardData.TypeCardParameter].r,
                cardData.TypeCardColor[(int)cardData.TypeCardParameter].g,
                cardData.TypeCardColor[(int)cardData.TypeCardParameter].b);

            _applyButton.onClick.AddListener(TakeCard);
            CreateParameterField();

            if (cardData.SuppurtivData.Count > 0)
                CreateSupportivParametrField();
        }

        private void TakeCard()
        {
            CardTaked?.Invoke(this);
        }

        private void CreateParameterField()
        {
            if (_cardData.AttributeData != null)
            {
                for (int index = 0; index < _cardData.AttributeData.CardParameters[_cardState.CurrentLevel].CardParameters.Count; index++)
                {
                    CardParameterView view = Instantiate(_cardData.AttributeData.CardParameterView, _cardParameterContainer);
                    _cardParametersViews.Add(view);

                    view.Initialize(
                        _cardData.AttributeData.CardParameters[_cardState.CurrentLevel].CardParameters[index].Value,
                        _cardData.AttributeData.CardParameters[_cardState.CurrentLevel].CardParameters[index].TypeParameter);
                }
            }
            else
            {
                LegendaryAbilityData legendaryAbilityData = (_cardData.AttributeData as AbilityAttributeData).LegendaryAbilityData;

                for (int index = 0; index < legendaryAbilityData.LegendaryAbilityParameters[_cardState.CurrentLevel].CardParameters.Count; index++)
                {
                    CardParameterView view = Instantiate(legendaryAbilityData.CardParameterView, _cardParameterContainer);
                    _cardParametersViews.Add(view);

                    view.Initialize(
                        legendaryAbilityData.LegendaryAbilityParameters[_cardState.CurrentLevel].CardParameters[index].Value,
                        legendaryAbilityData.LegendaryAbilityParameters[_cardState.CurrentLevel].CardParameters[index].TypeParameter);
                }
            }
        }

        private void CreateSupportivParametrField()
        {
            for (int i = 0; i < _cardData.SuppurtivData.Count; i++)
            {
                CardSupportivParametrsView view = Instantiate(_supportivParametrsView, _supportiParametrContainer);

                view.Initialize(_cardData.SuppurtivData[i].Icon);
            }
        }
    }
}