using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.Card
{
    public class ProcessPassiveCard : IProcessCard
    {
        public void ProcessCard(CardData cardData, CardState cardState)
        {
            if (cardState.CurrentLevel >= cardData.AttributeData.Parameters.Count)
            {
                cardState.SetCardLocked(true);
            }
        }
    }
}