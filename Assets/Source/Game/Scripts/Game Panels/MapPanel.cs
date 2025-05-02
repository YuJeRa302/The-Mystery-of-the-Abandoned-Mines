using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
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

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            base.Initialize(gamePanelsViewModel);
            //_miniMapRenderCamera.gameObject.SetActive(false);
            _openButton.onClick.AddListener(Open);
            _closeButton.onClick.AddListener(Close);
        }

        protected override void Open()
        {
            base.Open();
            //_miniMapRenderCamera.gameObject.SetActive(true);
        }

        protected override void Close()
        {
            base.Close();
            //_miniMapRenderCamera.gameObject.SetActive(false);
        }
    }
}