using Assets.Source.Game.Scripts;
using UnityEngine;

public class KnowleadgeCardParameterView : MonoBehaviour
{
    [SerializeField] private Transform _conteiner;

    public void Initialize(CardData cardData, int currentLvl)
    {
        if (cardData.AttributeData != null)
        {
            for (int index = 0; index < cardData.AttributeData.CardParameters[currentLvl].CardParameters.Count; index++)
            {
                CardParameterView view = Instantiate(cardData.AttributeData.CardParameterView, _conteiner);

                view.Initialize(
                    cardData.AttributeData.CardParameters[currentLvl].CardParameters[index].Value,
                    cardData.AttributeData.CardParameters[currentLvl].CardParameters[index].TypeParameter);
            }
        }
        else
        {
            //LegendaryAbilityData legendaryAbilityData = (cardData.AttributeData as AbilityAttributeData).LegendaryAbilityData;

            for (int index = 0; index < cardData.LegendaryAbilityData.LegendaryAbilityParameters[currentLvl].CardParameters.Count; index++)
            {
                CardParameterView view = Instantiate(cardData.LegendaryAbilityData.CardParameterView, _conteiner);

                view.Initialize(
                    cardData.LegendaryAbilityData.LegendaryAbilityParameters[currentLvl].CardParameters[index].Value,
                    cardData.LegendaryAbilityData.LegendaryAbilityParameters[currentLvl].CardParameters[index].TypeParameter);
            }
        }
    }
}