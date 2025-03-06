using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class LevelDataView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private readonly int _firstLevelIndex = 0;
        private readonly int _levelIndexShift = 1;

        [SerializeField] private Image _icon;
        [SerializeField] private Image _lockImage;
        [SerializeField] private LeanLocalizedText _name;
        [SerializeField] private Button _button;

        private LevelData _levelData;
        private LevelState _leveState;
        private IAudioPlayerService _audioPlayerService;
        private LevelsViewModel _levelsViewModel;

        public event Action<LevelDataView> LevelSelected;

        public LevelData LevelData => _levelData;

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelected);
        }

        public void Initialize(LevelData levelData, LevelState levelState, LevelsViewModel levelsViewModel, IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            _leveState = levelState;
            _levelData = levelData;
            _levelsViewModel = levelsViewModel;
            _button.onClick.AddListener(OnSelected);
            Fill(levelData);

            if (levelData.IsContractLevel == false)
            {
                LoadCompletePlayerLevels(levelState);
                CheckLevelState(levelState);
            }
            else 
            {
                SetLevelState(true);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //_audioPlayerService.PlayOneShotButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void Fill(LevelData levelData)
        {
            _icon.sprite = levelData.Icon;
            _name.TranslationName = levelData.TranslationName;
            _icon.color = new Color(levelData.TierColor.r, levelData.TierColor.g, levelData.TierColor.b);
        }

        private void LoadCompletePlayerLevels(LevelState levelState)
        {
            bool state = _levelsViewModel.GetLevels().Length > _firstLevelIndex ? _levelsViewModel.GetLevels()[levelState.Id].IsComplete : false;
            SetLevelState(state);

            if (levelState.Id == _firstLevelIndex)
                SetLevelState(true);
        }

        private void CheckLevelState(LevelState levelState)
        {
            if (levelState.Id == _firstLevelIndex)
                return;

            if (_levelsViewModel.GetLevels().Length > _firstLevelIndex)
                SetLevelState(_levelsViewModel.GetLevels()[levelState.Id - _levelIndexShift].IsComplete);
        }

        private void SetLevelState(bool isLevelComplete)
        {
            _lockImage.gameObject.SetActive(!isLevelComplete);
        }

        private void OnSelected()
        {
            if (_lockImage.isActiveAndEnabled == true)
                return;

            LevelSelected?.Invoke(this);
        }
    }
}