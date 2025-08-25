using Assets.Source.Game.Scripts.Models;
using Lean.Localization;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.GamePanels
{
    public class StagePanel : GamePanelsView
    {
        [SerializeField] private Button _buttonNext;
        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonExit;
        [Space(20)]
        [SerializeField] private LeanLocalizedText _titleText;
        [SerializeField] private Text _numberStageText;
        [Space(20)]
        [SerializeField] private Text _playerHealth;
        [SerializeField] private Text _playerDamage;
        [SerializeField] private Text _coins;
        [SerializeField] private Text _rerollPoints;
        [Space(10)]
        [SerializeField] private Text _currentRoomLevel;
        [SerializeField] private Text _killCount;

        private void Awake()
        {
            _buttonNext.onClick.AddListener(ClickButtonNext);
        }

        private void OnDestroy()
        {
            _buttonNext.onClick.RemoveListener(ClickButtonNext);
        }

        public override void Initialize(GamePanelsModel gamePanelsModel)
        {
            base.Initialize(gamePanelsModel);

            MessageBroker.Default
              .Receive<M_StageComplet>()
              .Subscribe(m => Open())
              .AddTo(Disposable);
        }

        protected override void Open()
        {
            base.Open();
            FillGameParameters();
        }

        private void FillGameParameters()
        {
            _numberStageText.text =
                GamePanelsModel.GetCurrentRoomLevel().ToString() +
                " / " + GamePanelsModel.GetStagesCount().ToString();

            _playerHealth.text = GamePanelsModel.GetPlayer().CurrentHealth.ToString();
            _playerDamage.text = GamePanelsModel.GetPlayer().DamageSource.Damage.ToString();
            _coins.text = GamePanelsModel.GetPlayer().Coins.ToString();
            _rerollPoints.text = GamePanelsModel.GetPlayer().PlayerStats.RerollPoints.ToString();
            _currentRoomLevel.text = GamePanelsModel.GetCurrentRoomLevel().ToString();
            _killCount.text = GamePanelsModel.GetPlayer().PlayerStats.CountKillEnemy.ToString();
        }

        private void ClickButtonNext()
        {
            Close();
        }
    }
}