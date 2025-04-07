using System;
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
        [SerializeField] private Button _buttonTest;
        //[SerializeField] private Button _buttonSkip;

        private List<CardView> _cardViews = new();

        private void OnDestroy()
        {
            GamePanelsViewModel.CardPoolCreated -= Fill;
            _buttonReroll.onClick.RemoveListener(Reroll);
            _buttonTest.onClick.RemoveListener(Open);
            //_buttonSkip.onClick.RemoveListener(Skip);
        }

        public void OpenCard()
        {
            Open();
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            _buttonReroll.onClick.AddListener(Reroll);
            _buttonTest.onClick.AddListener(Open);
            //_buttonSkip.onClick.AddListener(Skip);
            GamePanelsViewModel.CardPoolCreated += Fill;
        }

        protected override void Open()
        {
            GamePanelsViewModel.CreateCardPool();
            base.Open();

            if (GamePanelsViewModel.GetPlayer().PlayerStats.RerollPoint > 0)
                _buttonReroll.gameObject.SetActive(true);
        }

        protected override void Close()
        {
            GamePanelsViewModel.GetPlayer().UpdateDeck();
            Clear();
            base.Close();
        }

        private void Fill()
        {
            foreach (CardData cardData in GamePanelsViewModel.GetMainCardPool)
            {
                CardView view = Instantiate(_cardView, _cardContainer);
                _cardViews.Add(view);
                view.Initialize(GamePanelsViewModel.GetPlayer().GetCardStateByData(cardData), cardData);
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
            GamePanelsViewModel.GetPlayer().TakeCard(cardView);
            Close();
        }

        private void Skip()
        {
            Close();
        }

        private void Reroll()
        {
            if (GamePanelsViewModel.GetPlayer().TryGetRerollPoints())
            {
                Clear();
                GamePanelsViewModel.GetPlayer().UpdateDeck();
                GamePanelsViewModel.GetPlayer().UpdateCardPanelByRerollPoints();
                GamePanelsViewModel.CreateCardPool();
            }

            _buttonReroll.gameObject.SetActive(GamePanelsViewModel.GetPlayer().TryGetRerollPoints());
        }
    }
}