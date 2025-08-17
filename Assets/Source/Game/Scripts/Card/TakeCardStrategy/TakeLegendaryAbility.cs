using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Card
{
    public class TakeLegendaryAbility : BaseTakeAbility
    {
        private List<CardData> _activeCardAbility;

        public override TypeCardParameter TypeCardParameter => TypeCardParameter.LegendaryAbility;

        public override void Construct(List<CardData> cardDataAbility, List<CardData> activeCardAbility)
        {
            _activeCardAbility = activeCardAbility;
        }

        public override void TakeCard(CardView cardView)
        {
            if (TryAddCard(cardView))
                return;

            CardDeck.MessageBroker.Publish(new M_TakeAbility { CardView = cardView });

            if (_activeCardAbility.Contains(cardView.CardData))
                return;

            _activeCardAbility.Add(cardView.CardData);
        }
    }
}