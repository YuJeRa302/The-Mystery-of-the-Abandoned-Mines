using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Source.Game.Scripts
{
    public class CardPanel : GamePanelsView
    {
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private CardView _cardView;
        [Space(20)]
        [SerializeField] private Button _buttonReroll;
        [SerializeField] private Button _buttonAds;
        [SerializeField] private Button _buttonTest;
        [SerializeField] private Text _countRerollPointsText;

        private List<CardView> _cardViews = new();

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            AddListeners();
        }

        protected override void Open()
        {
            GamePanelsViewModel.CreateCardPool();
            base.Open();

            if (GamePanelsViewModel.GetPlayer().RerollPoints > 0)
                _buttonReroll.gameObject.SetActive(true);

            _countRerollPointsText.text = GamePanelsViewModel.GetPlayer().RerollPoints.ToString();
        }

        protected override void Close()
        {
            GamePanelsViewModel.GetPlayer().UpdateDeck();
            Clear();
            base.Close();
        }

        protected override void OpenRewardAds()
        {
            base.OpenRewardAds();
            YandexGame.RewVideoShow(1);
        }

        private void OnRewardCallback(int index)
        {
            GamePanelsViewModel.GetRerollPointsReward();
        }

        private void OnCardTaked(CardView cardView)
        {
            GamePanelsViewModel.GetPlayer().TakeCard(cardView);
            Close();
        }

        private void OnCloseAdCallback()
        {
            RewardAdClosed?.Invoke();
            _countRerollPointsText.text = GamePanelsViewModel.GetPlayer().RerollPoints.ToString();
            _buttonReroll.gameObject.SetActive(GamePanelsViewModel.GetPlayer().TryGetRerollPoints());
        }

        private void OnErrorCallback()
        {
            CloseRewardAds();
        }

        private void AddListeners()
        {
            _buttonReroll.onClick.AddListener(Reroll);
            _buttonTest.onClick.AddListener(Open);
            _buttonAds.onClick.AddListener(OpenRewardAds);
            GamePanelsViewModel.CardPoolCreated += Fill;
            GamePanelsViewModel.CardPanelOpened += Open;
            YandexGame.RewardVideoEvent += OnRewardCallback;
            YandexGame.CloseVideoEvent += OnCloseAdCallback;
            YandexGame.ErrorVideoEvent += OnErrorCallback;
        }

        private void RemoveListeners()
        {
            _buttonReroll.onClick.RemoveListener(Reroll);
            _buttonTest.onClick.RemoveListener(Open);
            _buttonAds.onClick.RemoveListener(OpenRewardAds);
            GamePanelsViewModel.CardPanelOpened -= Open;
            GamePanelsViewModel.CardPoolCreated -= Fill;
            YandexGame.RewardVideoEvent -= OnRewardCallback;
            YandexGame.CloseVideoEvent -= OnCloseAdCallback;
            YandexGame.ErrorVideoEvent -= OnErrorCallback;
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

        private void Reroll()
        {
            if (GamePanelsViewModel.GetPlayer().TryGetRerollPoints())
            {
                Clear();
                GamePanelsViewModel.GetPlayer().UpdateDeck();
                GamePanelsViewModel.GetPlayer().UpdateCardPanelByRerollPoints();
                GamePanelsViewModel.CreateCardPool();
                _countRerollPointsText.text = GamePanelsViewModel.GetPlayer().RerollPoints.ToString();
            }

            _buttonReroll.gameObject.SetActive(GamePanelsViewModel.GetPlayer().TryGetRerollPoints());
        }
    }
}