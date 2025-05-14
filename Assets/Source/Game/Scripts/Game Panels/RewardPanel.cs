using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using YG;

namespace Assets.Source.Game.Scripts
{
    public class RewardPanel : GamePanelsView
    {
        private readonly int _defaultAdId = 0;
        private readonly int _multiplier = 2;
        private readonly float _animationDuration = 1f;

        [SerializeField] private Button _openAdButton;
        [SerializeField] private Button _collectButton;
        [SerializeField] private Button _closeGameButton;
        [SerializeField] private Button _testAdsButton;
        [Space(20)]
        [SerializeField] private Image _weaponIcon;
        [SerializeField] private Image _weaponBackgroundIcon;
        [Space(20)]
        [SerializeField] private Text _coinsText;
        [SerializeField] private Text _upgradePointsText;
        [SerializeField] private Text _defaultContractRewardCoins;
        [Space(20)]
        [SerializeField] private GameObject _defaulContractReward;
        [SerializeField] private GameObject _weaponContractReward;
        [SerializeField] private GameObject _levelCompleteReward;
        [Space(20)]
        [SerializeField] private Sprite _loseGameSprite;
        [SerializeField] private Sprite _winGameSprite;
        [SerializeField] private Text _gameStateText;
        [SerializeField] private Image _imageGameState;

        private WeaponData _currentWeaponData;

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            AddListeners();
        }

        protected override void OpenRewardAds()
        {
            base.OpenRewardAds();
            YandexGame.RewVideoShow(_defaultAdId);
        }

        protected override void CloseGame()
        {
            base.CloseGame();
            YandexGame.FullscreenShow();
        }

        private void OpenRewardPanel(bool gameState)
        {
            CreateViewEntities(gameState);
            base.Open();
        }

        private void ShowReward(int reward)
        {
            base.Open();
            //_defaulReward.SetActive(true);
            Fill();
            //_defaultRewardCoins.text = reward.ToString();
        }
        
        private void OnRewardCallback(int index)
        {
            Debug.Log("Reward");
        }

        private void OnOpenFullscreenAdCallback()
        {
            OpenFullscreenAds();
        }

        private void OnCloseFullscreenAdCallback()
        {
            CloseFullscreenAds();
        }

        private void OnCloseAdCallback()
        {
            RewardAdClosed?.Invoke();
            int coinRewards = GamePanelsViewModel.GetPlayer().Coins * _multiplier;
            int upgradePointRewards = GamePanelsViewModel.GetPlayer().UpgradePoints * _multiplier;

            DOVirtual.Int(GamePanelsViewModel.GetPlayer().Coins, coinRewards, _animationDuration, (value) =>
            {
                _coinsText.text = value.ToString();
            });

            DOVirtual.Int(GamePanelsViewModel.GetPlayer().UpgradePoints, upgradePointRewards, _animationDuration, (value) =>
            {
                _upgradePointsText.text = value.ToString();
            });

            _openAdButton.gameObject.SetActive(false);
            _collectButton.gameObject.SetActive(false);
            _closeGameButton.gameObject.SetActive(true);
        }

        private void OnErrorCallback()
        {
            CloseRewardAds();
        }

        private void AddListeners()
        {
            GamePanelsViewModel.GameEnded += OpenRewardPanel;
            GamePanelsViewModel.LootRoomComplitetd += ShowReward;
            YandexGame.RewardVideoEvent += OnRewardCallback;
            YandexGame.CloseVideoEvent += OnCloseAdCallback;
            YandexGame.ErrorVideoEvent += OnErrorCallback;
            YandexGame.OpenFullAdEvent += OnOpenFullscreenAdCallback;
            YandexGame.CloseFullAdEvent += OnCloseFullscreenAdCallback;
            _closeGameButton.onClick.AddListener(CloseGame);
            _openAdButton.onClick.AddListener(OpenRewardAds);

            _testAdsButton.onClick.AddListener(OnCloseAdCallback); // test Ads button
        }

        private void RemoveListeners()
        {
            GamePanelsViewModel.GameEnded -= OpenRewardPanel;
            YandexGame.RewardVideoEvent -= OnRewardCallback;
            YandexGame.CloseVideoEvent -= OnCloseAdCallback;
            YandexGame.ErrorVideoEvent -= OnErrorCallback;
            YandexGame.OpenFullAdEvent -= OnOpenFullscreenAdCallback;
            YandexGame.CloseFullAdEvent -= OnCloseFullscreenAdCallback;
            _closeGameButton.onClick.RemoveListener(CloseGame);
            _openAdButton.onClick.RemoveListener(OpenRewardAds);
        }

        private void CreateViewEntities(bool gameState) 
        {
            if (GamePanelsViewModel.IsContractLevel == true)
                CreateWeaponRewards(gameState);
            else
                CreateDefaultRewards(gameState);
        }

        private void CreateWeaponRewards(bool gameState) 
        {
            if (gameState == true)
            {
                _currentWeaponData = GamePanelsViewModel.CreateRewardWeapon();

                if (_currentWeaponData != null)
                {
                    _weaponContractReward.SetActive(true);
                    _weaponIcon.sprite = _currentWeaponData.Icon;
                    _weaponBackgroundIcon.color = new Color(_currentWeaponData.TierColor.r, _currentWeaponData.TierColor.g, _currentWeaponData.TierColor.b);
                    Fill();
                }
                else
                {
                    _defaulContractReward.SetActive(true);
                    CreateCoinsRewards();
                }
            }
            else 
            {
                CreateEndScreen(gameState);
            }
        }

        private void CreateDefaultRewards(bool gameState)
        {
            CreateEndScreen(gameState);
            Fill();
        }

        private void CreateEndScreen(bool gameState) 
        {
            _levelCompleteReward.SetActive(true);

            if (gameState == true)
            {
                _imageGameState.sprite = _winGameSprite;
                _gameStateText.text = "Win";
            }
            else 
            {
                _imageGameState.sprite = _loseGameSprite;
                _gameStateText.text = "Lose";
            }
        }

        private void CreateCoinsRewards()
        {
            _defaulContractReward.SetActive(true);
            _defaultContractRewardCoins.text = GamePanelsViewModel.GetPlayer().Coins.ToString();
            Fill();
        }

        private void Fill() 
        {
            _coinsText.text = GamePanelsViewModel.GetPlayer().Coins.ToString();
            _upgradePointsText.text = GamePanelsViewModel.GetPlayer().UpgradePoints.ToString();
        }
    }
}