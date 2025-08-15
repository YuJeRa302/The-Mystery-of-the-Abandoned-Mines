using Assets.Source.Game.Scripts.ScriptableObjects;

namespace Assets.Source.Game.Scripts.Card
{
    public interface IProcessCard
    {
        public void ProcessCard(CardData cardData, CardState cardState);
    }
}