using Assets.Source.Game.Scripts.Enums;

namespace Assets.Source.Game.Scripts.Card
{
    public class TakePrimaryAttribute : BaseTakeAbility
    {
        public override TypeCardParameter TypeCardParameter => TypeCardParameter.Default;

        public override void TakeCard(CardView cardView)
        {
            if (TryAddCard(cardView))
                return;

            CardDeck.MessageBroker.Publish(new M_TakePrimaryAttribute { CardView = cardView });
        }
    }
}