using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts.Card
{
    public class TakeDefaultAbility : BaseTakeAbility
    {
        private List<CardData> _cardDataAbility;
        private List<CardData> _activeCardAbility;

        public override TypeCardParameter TypeCardParameter => TypeCardParameter.Ability;

        public override void Construct(List<CardData> cardDataAbility, List<CardData> activeCardAbility)
        {
            _cardDataAbility = cardDataAbility;
            _activeCardAbility = activeCardAbility;
        }

        public override void TakeCard(CardView cardView)
        {
            if (TryAddCard(cardView))
                return;

            CardDeck.MessageBroker.Publish(new M_TakeAbility { CardView = cardView });

            if(cardView.CardData.AttributeData is AttackAbilityData)
                AddCardAbilityData(cardView, _activeCardAbility);
            else
                AddCardAbilityData(cardView, _cardDataAbility);
        }

        private void AddCardAbilityData(CardView cardView, List<CardData> repository)
        {
            if (repository.Contains(cardView.CardData))
                return;

            repository.Add(cardView.CardData);
        }
    }
}