using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class RewardPanel : GamePanelsView
    {
        [SerializeField] private Button _openAdButton;
        [SerializeField] private Button _closeButton;
        [Space(20)]
        [SerializeField] private Image _weaponIcon;
        [SerializeField] private Image _weaponBackgroundIcon;
        [Space(20)]
        [SerializeField] private Text _coins;
        [SerializeField] private Text _killCount;
        [SerializeField] private Text _defaultRewardCoins;
        [Space(20)]
        [SerializeField] private GameObject _defaulReward;
        [SerializeField] private GameObject _contractReward;

        private WeaponData _currentWeaponData;

        private void OnDestroy()
        {
            GamePanelsViewModel.GameEnded -= Open;
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            GamePanelsViewModel.GameEnded += Open;
        }

        protected override void Open()
        {
            CreateViewEntities();
            base.Open();
        }

        protected override void Close()
        {
            base.Close();
        }

        private void CreateViewEntities() 
        {
            if (GamePanelsViewModel.GetLevelType() == true)
                CreateContractRewards();
            else
                CreateDefaultRewards();
        }

        private void CreateContractRewards() 
        {
            _currentWeaponData = GamePanelsViewModel.CreateRewardWeapon();

            if (_currentWeaponData != null)
            {
                _contractReward.SetActive(true);
                _weaponIcon.sprite = _currentWeaponData.Icon;
                _weaponBackgroundIcon.color = new Color(_currentWeaponData.TierColor.r, _currentWeaponData.TierColor.g, _currentWeaponData.TierColor.b);
                Fill();
            }
            else 
            {
                CreateDefaultRewards();
            }
        }

        private void CreateDefaultRewards()
        {
            _defaulReward.SetActive(true);
            //_defaultRewardCoins.text = GamePanelsViewModel;
            Fill();
        }

        private void Fill() 
        {
            //_killCount.text = GamePanelsViewModel.GetPlayer();
            //_coins.text = GamePanelsViewModel.GetPlayer();
        }
    }
}