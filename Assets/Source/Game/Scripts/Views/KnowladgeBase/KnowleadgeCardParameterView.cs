using Assets.Source.Game.Scripts;
using UnityEngine;

public class KnowleadgeCardParameterView : MonoBehaviour
{
    [SerializeField] private Transform _container;

    public void Initialize(CardData cardData, int currentLvl)
    {
        for (int index = 0; index < cardData.AttributeData.Parameters[currentLvl].CardParameters.Count; index++)
        {
            CardParameterView view = Instantiate(cardData.AttributeData.ParameterView, _container);

            view.Initialize(
                cardData.AttributeData.Parameters[currentLvl].CardParameters[index].Value,
                cardData.AttributeData.Parameters[currentLvl].CardParameters[index].TypeParameter);
        }
    }
}