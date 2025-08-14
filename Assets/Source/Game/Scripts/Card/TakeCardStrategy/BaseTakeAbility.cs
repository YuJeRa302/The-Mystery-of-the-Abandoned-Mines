using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Card
{
    public abstract class BaseTakeAbility : ITakeCardStrategy
    {
        public abstract TypeCardParameter TypeCardParameter { get; }

        public virtual void Construct(List<CardData> cardDataAbility, List<CardData> activeCardAbility)
        {
        }

        public virtual void TakeCard(CardView cardView)
        {
        }

        public bool TryAddCard(CardView cardView)
        {
            return cardView.CardData.TypeCardParameter != TypeCardParameter;
        }
    }
}