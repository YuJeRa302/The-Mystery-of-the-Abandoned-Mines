using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using YG;
using Lean.Localization;

namespace Assets.Source.Game.Scripts
{
    public class RewardPanel : GamePanelsView
    {
        private readonly float _animationDuration = 1f;

        [SerializeField] private Button _openAdButton;
        [SerializeField] private Button _collectButton;
        [SerializeField] private Button _closeGameButton;
        [SerializeField] private Button _applayReward;
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
        [SerializeField] private LeanLocalizedText _gameStateText;
        [SerializeField] private Image _imageGameState;

        private bool _isRewardReceived = false;
        private WeaponData _currentWeaponData;
        private bool _isLootReward = false;
        private int _currentRewardLoot;

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
            YandexGame.RewVideoShow(0);
        }

        protected override void CloseGame()
        {
            base.CloseGame();
            _collectButton.onClick.RemoveListener(CloseGame);
            YandexGame.FullscreenShow();
        }

        private void OpenRewardPanel(bool gameState)
        {
            _collectButton.onClick.AddListener(CloseGame);
            CreateViewEntities(gameState);
            base.Open();
        }

        private void ShowReward(int reward)
        {
            CreateLootRoomRevard(reward);
            base.Open();
        }
        
        private void OnRewardCallback(int index)
        {
            GamePanelsViewModel.GetEndGameReward();
            _isRewardReceived = true;
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
            if (_isLootReward)
            {
                int cointReward = _currentRewardLoot + _currentRewardLoot;
                SetAnimationText(_currentRewardLoot, cointReward, _defaultContractRewardCoins);
                GamePanelsViewModel.GetLootRoomRewaed(_currentRewardLoot);
                _openAdButton.gameObject.SetActive(false);
                _collectButton.gameObject.SetActive(false);
                _applayReward.gameObject.SetActive(true);
            }
            else
            {
                if (_isRewardReceived == true) 
                {
                    int coinRewards = GamePanelsViewModel.GetPlayer().Coins + GamePanelsViewModel.GetPlayer().Coins;
                    int upgradePointRewards = GamePanelsViewModel.GetPlayer().UpgradePoints + GamePanelsViewModel.GetPlayer().UpgradePoints;
                    SetAnimationText(GamePanelsViewModel.GetPlayer().Coins, coinRewards, _coinsText);
                    SetAnimationText(GamePanelsViewModel.GetPlayer().UpgradePoints, upgradePointRewards, _upgradePointsText);
                    _openAdButton.gameObject.SetActive(false);
                    _collectButton.gameObject.SetActive(false);
                    _closeGameButton.gameObject.SetActive(true);
                }
            }
                RewardAdClosed?.Invoke();
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
            _applayReward.onClick.AddListener(PlayerApplayReward);
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
            _applayReward.onClick.RemoveListener(PlayerApplayReward);
            _openAdButton.onClick.RemoveListener(OpenRewardAds);
        }

        private void PlayerApplayReward()
        {
            _collectButton.onClick.RemoveListener(PlayerApplayReward);
            _isLootReward = false;
            _currentRewardLoot = 0;
            _openAdButton.gameObject.SetActive(true);
            _collectButton.gameObject.SetActive(true);
            _applayReward.gameObject.SetActive(false);
            _defaulContractReward.SetActive(false);
            Close();
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

        private void SetAnimationText(int cuurentValue, int rewardValue, Text text) 
        {
            DOVirtual.Int(cuurentValue, rewardValue, _animationDuration, (value) =>
            {
                text.text = value.ToString();
            });
        }

        private void CreateDefaultRewards(bool gameState)
        {
            CreateEndScreen(gameState);
            Fill();
        }

        private void CreateEndScreen(bool gameState) 
        {
            _levelCompleteReward.SetActive(true);
            _imageGameState.sprite = gameState == true ? _winGameSprite : _loseGameSprite;
            _gameStateText.TranslationName = gameState == true ? "Win" : "Lose";
        }

        private void CreateCoinsRewards()
        {
            _defaulContractReward.SetActive(true);
            _defaultContractRewardCoins.text = GamePanelsViewModel.GetPlayer().Coins.ToString();
            Fill();
        }

        private void CreateLootRoomRevard(int reward)
        {
            _isLootReward = true;
            _defaulContractReward.SetActive(true);
            _currentRewardLoot = reward;
            _defaultContractRewardCoins.text = reward.ToString();
            _collectButton.onClick.AddListener(PlayerApplayReward);
            Fill();
        }

        private void Fill() 
        {
            _coinsText.text = GamePanelsViewModel.GetPlayer().Coins.ToString();
            _upgradePointsText.text = GamePanelsViewModel.GetPlayer().UpgradePoints.ToString();
        }
    }
}