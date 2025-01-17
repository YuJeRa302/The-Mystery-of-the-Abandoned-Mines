using System.Collections.Generic;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class CardPanel : GamePanels
    {
        private readonly string _skipTextTranslationName = "Skip";
        private readonly string _rerollTextTranslationName = "Reroll";

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

        private Player _player;
        private List<CardView> _cardViews = new();

        private void Awake()
        {
            gameObject.SetActive(false);
            _buttonTest.onClick.AddListener(Open);
            _buttonReroll.onClick.AddListener(Reroll);
            _buttonSkip.onClick.AddListener(Skip);
            _cardLoader.CardPoolCreated += Fill;
            //_buttonSkipText.TranslationName = _skipTextTranslationName;
            //_buttonRerollText.TranslationName = _rerollTextTranslationName;
        }

        private void OnDestroy()
        {
            _buttonReroll.onClick.RemoveListener(Reroll);
            _buttonSkip.onClick.RemoveListener(Skip);
            _cardLoader.CardPoolCreated -= Fill;
        }

        public void Initialize(Player player)
        {
            _player = player;
            _cardLoader.Initialize(_player.CardDeck);
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
                view.Initialize(_cardLoader.GetCardStateById(cardData.Id), cardData);
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
            _player.CardDeck.TakeCard(cardView);
            Close();
        }

        private void Skip()
        {
            Close();
        }

        private void Reroll()
        {
            if (_player.PlayerStats.TryGetRerollPoints())
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
