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
        [SerializeField] private Sprite _star;
        [SerializeField] private Sprite _complitStar;
        [SerializeField] private Image _starLvlPrefab;
        [Space(10)]
        [SerializeField] private Image _bgContractInfo;
        [SerializeField] private Text _price;

        private LevelData _levelData;
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
            _levelData = levelData;
            _levelsViewModel = levelsViewModel;
            _button.onClick.AddListener(OnSelected);
            Fill(levelData);
            UpdateLevelState(levelData, levelState);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void Fill(LevelData levelData)
        {
            _icon.sprite = levelData.Icon;
            _name.TranslationName = levelData.TranslationName;
            _icon.color = new Color(levelData.TierColor.r, levelData.TierColor.g, levelData.TierColor.b);
        }

        private void UpdateLevelState(LevelData levelData, LevelState levelState) 
        {
            if (levelData.IsContractLevel == false)
                UpdateDefaultLevelState(levelState);
            else
                UpdateContractLevelState(levelData);
        }

        private void UpdateDefaultLevelState(LevelState levelState)
        {
            if (levelState.Id == _firstLevelIndex) 
            {
                SetImageLevelState(true);
                return;
            }

            if (_levelsViewModel.GetLevels().Length > _firstLevelIndex)
                SetImageLevelState(_levelsViewModel.GetLevels()[levelState.Id - _levelIndexShift].IsComplete);
        }

        private void UpdateContractLevelState(LevelData levelData)
        {
            _bgContractInfo.gameObject.SetActive(true);
            _price.text = levelData.Cost.ToString();
            SetImageLevelState(true);
        }

        private void SetImageLevelState(bool isLevelComplete)
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