using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    public class KnowledgeCardParameterView : MonoBehaviour
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
}