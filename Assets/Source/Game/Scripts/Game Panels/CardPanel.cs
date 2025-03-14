using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class CardPanel : GamePanelsView
    {
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private CardView _cardView;
        [Space(20)]
        [SerializeField] private Button _buttonReroll;
        //[SerializeField] private Button _buttonSkip;

        private List<CardView> _cardViews = new();

        private void Awake()
        {
            //_buttonSkip.onClick.AddListener(Skip);
            _buttonTest.onClick.AddListener(Open);
            _buttonReroll.onClick.AddListener(Reroll);
            GamePanelsViewModel.CardPoolCreated += Fill;
        }

        private void OnDestroy()
        {
            _buttonReroll.onClick.RemoveListener(Reroll);
            //_buttonSkip.onClick.RemoveListener(Skip);
        }

        public override void Initialize(Player player, LevelObserver levelObserver)
        {
            base.Initialize(player, levelObserver);
            GamePanelsViewModel.CardPoolCreated -= Fill;
        }

        public void OpenCard()
        {
            Open();
        }

        protected override void Open()
        {
            GamePanelsViewModel.CreateCardPool();
            base.Open();

            if (Player.PlayerStats.RerollPoint > 0)
                _buttonReroll.gameObject.SetActive(true);
        }

        protected override void Close()
        {
            Clear();
            base.Close();
        }

        private void Fill()
        {
            foreach (CardData cardData in GamePanelsViewModel.GetMainCardPool)
            {
                CardView view = Instantiate(_cardView, _cardContainer);
                _cardViews.Add(view);
                view.Initialize(GamePanelsViewModel.GetPlayer().CardDeck.GetCardStateByData(cardData), cardData);
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
            GamePanelsViewModel.GetPlayer().CardDeck.TakeCard(cardView);
            Close();
        }

        private void Skip()
        {
            Close();
        }

        private void Reroll()
        {
            // if (Player.PlayerStats.TryGetRerollPoints(out bool canNextReroll))
            // {
            //     Clear();
            //     _cardLoader.CreateCardPool();

            //     if (canNextReroll == false)
            //         _buttonReroll.gameObject.SetActive(false);
            // }
            // else
            if (GamePanelsViewModel.GetPlayer().PlayerStats.TryGetRerollPoints() == false)
            {
                _buttonReroll.gameObject.SetActive(false);
            }
            else 
            {
                Clear();
                GamePanelsViewModel.CreateCardPool();
            }
        }
    }
}
