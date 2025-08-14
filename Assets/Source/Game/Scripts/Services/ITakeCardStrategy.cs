using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Services
{
    public interface ITakeCardStrategy
    {
        public void Construct(List<CardData> cardDataAbility, List<CardData> activeCardAbility);
        public void TakeCard(CardView cardView);
    }
}