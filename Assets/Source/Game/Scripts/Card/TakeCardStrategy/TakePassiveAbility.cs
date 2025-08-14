using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Card
{
    public class TakePassiveAbility : BaseTakeAbility
    {
        private List<CardData> _cardDataAbility;

        public override TypeCardParameter TypeCardParameter => TypeCardParameter.PassiveAbility;

        public override void Construct(List<CardData> cardDataAbility, List<CardData> activeCardAbility)
        {
            _cardDataAbility = cardDataAbility;
        }

        public override void TakeCard(CardView cardView)
        {
            if (TryAddCard(cardView))
                return;

            CardDeck.MessageBroker.Publish(new M_TakePassiveAbility { CardView = cardView });

            if (_cardDataAbility.Contains(cardView.CardData))
                return;

            _cardDataAbility.Add(cardView.CardData);
        }
    }
}