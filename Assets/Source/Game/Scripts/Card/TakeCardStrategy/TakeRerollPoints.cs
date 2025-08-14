using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Game.Scripts.Card
{
    public class TakeRerollPoints : BaseTakeAbility
    {
        public override TypeCardParameter TypeCardParameter => TypeCardParameter.RerollPoints;

        public override void TakeCard(CardView cardView)
        {
            if (TryAddCard(cardView))
                return;

            CardDeck.MessageBroker.Publish(new M_TakeRerollPoints { CardView = cardView });
        }
    }
}