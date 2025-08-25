using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using System.Collections.Generic;
using UniRx;
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

        private List<CardView> _cardViews = new ();
        private GamePanelsModel _gamePanelsViewModel;
        private Player _player;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public override void Initialize(GamePanelsModel gamePanelsModel)
        {
            base.Initialize(gamePanelsModel);
            _gamePanelsViewModel = gamePanelsModel;
            AddListeners();
            _player = GamePanelsModel.GetPlayer();
        }

        protected override void Open()
        {
            GamePanelsModel.CreateCardPool();
            base.Open();

            if (_player.PlayerStats.RerollPoints > 0)
                _buttonReroll.gameObject.SetActive(true);

            _countRerollPointsText.text = _player.PlayerStats.RerollPoints.ToString();
        }

        protected override void Close()
        {
            _player.CardDeck.UpdateDeck();
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
                GamePanelsModel.GetRerollPointsReward();
        }

        private void OnCardTaked(CardView cardView)
        {
            _player.CardDeck.TakeCard(cardView);
            Close();
        }

        private void OnCloseAdCallback()
        {
            CloseRewardAds();
            _countRerollPointsText.text = _player.PlayerStats.RerollPoints.ToString();
            _buttonReroll.gameObject.SetActive(_player.PlayerStats.TryGetRerollPoints());
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

            MessageBroker.Default
              .Receive<M_CardPoolCreat>()
              .Subscribe(m => Fill())
              .AddTo(Disposable);

            MessageBroker.Default
              .Receive<M_CardPanelOpen>()
              .Subscribe(m => Open())
              .AddTo(Disposable);

            YG2.onRewardAdv += OnRewardCallback;
            YG2.onCloseRewardedAdv += OnCloseAdCallback;
            YG2.onErrorRewardedAdv += OnErrorCallback;
        }

        private void RemoveListeners()
        {
            _buttonReroll.onClick.RemoveListener(Reroll);
            _buttonTest.onClick.RemoveListener(Open);
            _buttonAds.onClick.RemoveListener(OpenRewardAds);
            YG2.onRewardAdv -= OnRewardCallback;
            YG2.onCloseRewardedAdv -= OnCloseAdCallback;
            YG2.onErrorRewardedAdv -= OnErrorCallback;
        }

        private void Fill()
        {
            foreach (CardData cardData in GamePanelsModel.GetMainCardPool())
            {
                CardView view = Instantiate(_cardView, _cardContainer);
                _cardViews.Add(view);
                view.Initialize(GamePanelsModel.GetPlayer().CardDeck.GetCardStateByData(cardData), cardData);
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
            if (_player.PlayerStats.TryGetRerollPoints())
            {
                Clear();
                _player.CardDeck.UpdateDeck();
                _player.PlayerStats.UpdateCardPanelByRerollPoints();
                GamePanelsModel.CreateCardPool();
                _countRerollPointsText.text = _player.PlayerStats.RerollPoints.ToString();
            }

            _buttonReroll.gameObject.SetActive(_player.PlayerStats.TryGetRerollPoints());
        }
    }
}