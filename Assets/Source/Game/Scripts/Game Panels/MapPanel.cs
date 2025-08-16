using Assets.Source.Game.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.GamePanels
{
    public class MapPanel : GamePanelsView
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Camera _miniMapRenderCamera;

        private void OnDestroy()
        {
            _openButton.onClick.RemoveListener(Open);
            _closeButton.onClick.RemoveListener(Close);
        }

        public override void Initialize(GamePanelsModel gamePanelsModel)
        {
            base.Initialize(gamePanelsModel);
            _openButton.onClick.AddListener(Open);
            _closeButton.onClick.AddListener(Close);
        }
    }
}