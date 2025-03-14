using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class MapPanel : GamePanelsView
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(Open);
            _closeButton.onClick.RemoveListener(Close);
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            _openButton.onClick.AddListener(Open);
            _closeButton.onClick.AddListener(Close);
        }
    }
}