using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.ViewModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Source.Game.Scripts.GamePanels
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
        private GamePanelsViewModel _gamePanelsViewModel;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            _gamePanelsViewModel = gamePanelsViewModel;
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
            GamePanelsViewModel.GetPlayer().CardDeck.UpdateDeck();
            Clear();
            base.Close();
        }

        protected override void OpenRewardAds()
        {
            base.OpenRewardAds();
            YG2.RewardedAdvShow(_gamePanelsViewModel.GetRerollPointsRewardIndex());
        }

        private void OnRewardCallback(string index)
        {
            if (index == _gamePanelsViewModel.GetRerollPointsRewardIndex())
                GamePanelsViewModel.GetRerollPointsReward();
        }

        private void OnCardTaked(CardView cardView)
        {
            GamePanelsViewModel.GetPlayer().CardDeck.TakeCard(cardView);
            Close();
        }

        private void OnCloseAdCallback()
        {
            CloseRewardAds();
            _countRerollPointsText.text = GamePanelsViewModel.GetPlayer().RerollPoints.ToString();
            _buttonReroll.gameObject.SetActive(GamePanelsViewModel.GetPlayer().PlayerStats.TryGetRerollPoints());
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
            YG2.onRewardAdv += OnRewardCallback;
            YG2.onCloseRewardedAdv += OnCloseAdCallback;
            YG2.onErrorRewardedAdv += OnErrorCallback;
        }

        private void RemoveListeners()
        {
            _buttonReroll.onClick.RemoveListener(Reroll);
            _buttonTest.onClick.RemoveListener(Open);
            _buttonAds.onClick.RemoveListener(OpenRewardAds);
            GamePanelsViewModel.CardPanelOpened -= Open;
            GamePanelsViewModel.CardPoolCreated -= Fill;
            YG2.onRewardAdv -= OnRewardCallback;
            YG2.onCloseRewardedAdv -= OnCloseAdCallback;
            YG2.onErrorRewardedAdv -= OnErrorCallback;
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

        private void Reroll()
        {
            if (GamePanelsViewModel.GetPlayer().PlayerStats.TryGetRerollPoints())
            {
                Clear();
                GamePanelsViewModel.GetPlayer().CardDeck.UpdateDeck();
                GamePanelsViewModel.GetPlayer().PlayerStats.UpdateCardPanelByRerollPoints();
                GamePanelsViewModel.CreateCardPool();
                _countRerollPointsText.text = GamePanelsViewModel.GetPlayer().RerollPoints.ToString();
            }

            _buttonReroll.gameObject.SetActive(GamePanelsViewModel.GetPlayer().PlayerStats.TryGetRerollPoints());
        }
    }
}