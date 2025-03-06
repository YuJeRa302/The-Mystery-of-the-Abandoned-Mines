using System.Collections.Generic;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class CardPanel : GamePanels
    {
        [SerializeField] private CardLoader _cardLoader;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private CardView _cardView;
        [SerializeField] private PlayerView _testPanel;
        [Space(20)]
        [SerializeField] private Button _buttonTest;
        [SerializeField] private Button _buttonReroll;
        [SerializeField] private Button _buttonSkip;
        [Space(20)]
        [SerializeField] private LeanLocalizedText _buttonSkipText;
        [SerializeField] private LeanLocalizedText _buttonRerollText;

        private List<CardView> _cardViews = new();

        private void Awake()
        {
            gameObject.SetActive(false);
            _buttonTest.onClick.AddListener(Open);
            _buttonReroll.onClick.AddListener(Reroll);
            _buttonSkip.onClick.AddListener(Skip);
            _cardLoader.CardPoolCreated += Fill;
        }

        private void OnDestroy()
        {
            _buttonReroll.onClick.RemoveListener(Reroll);
            _buttonSkip.onClick.RemoveListener(Skip);
            _cardLoader.CardPoolCreated -= Fill;
        }

        public override void Initialize(Player player, LevelObserver levelObserver)
        {
            base.Initialize(player, levelObserver);
            _cardLoader.Initialize(player.CardDeck);
        }

        protected override void Open()
        {
            _cardLoader.CreateCardPool();
            base.Open();
        }

        protected override void Close()
        {
            Clear();
            base.Close();
        }

        private void Fill()
        {
            foreach (CardData cardData in _cardLoader.MainCardsPool)
            {
                CardView view = Instantiate(_cardView, _cardContainer);
                _cardViews.Add(view);
                view.Initialize(Player.CardDeck.GetCardStateByData(cardData), cardData);
                view.CardTaked += OnCardTaked;
            }
        }

        private void Clear()
        {
            foreach (CardView view in _cardViews)
            {
                view.CardTaked -= OnCardTaked;
                Destroy(view.gameObject);
            }

            _cardViews.Clear();
        }

        private void OnCardTaked(CardView cardView)
        {
            Player.CardDeck.TakeCard(cardView);
            Close();
        }

        private void Skip()
        {
            Close();
        }

        private void Reroll()
        {
            if (Player.PlayerStats.TryGetRerollPoints())
            {
                Clear();
                _cardLoader.CreateCardPool();
            }
            else
            {
                _buttonReroll.gameObject.SetActive(false);
            }
        }
    }
}
