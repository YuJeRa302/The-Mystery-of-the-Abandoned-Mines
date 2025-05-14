using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class GamePanelsViewModel
    {
        private readonly GamePanelsModel _gamePanelsModel;
        
        public GamePanelsViewModel(GamePanelsModel gamePanelsModel)
        {
            _gamePanelsModel = gamePanelsModel;
            _gamePanelsModel.StageCompleted += () => StageCompleted?.Invoke();
            _gamePanelsModel.CardPoolCreated += () => CardPoolCreated?.Invoke();
            _gamePanelsModel.GameEnded += (bool state) => GameEnded?.Invoke(state);
            _gamePanelsModel.CardPanelOpened += () => CardPanelOpened?.Invoke();
        }

        public event Action StageCompleted;
        public event Action CardPoolCreated;
        public event Action CardPanelOpened;
        public event Action<bool> GameEnded;

        public bool IsContractLevel => _gamePanelsModel.GetLevelType();
        public WeaponData CreateRewardWeapon() => _gamePanelsModel.CreateRewardWeapon();
        public Player GetPlayer() => _gamePanelsModel.GetPlayer();
        public List<CardData> GetMainCardPool => _gamePanelsModel.GetMainCardPool();
        public void CreateCardPool() => _gamePanelsModel.CreateCardPool();
        public int GetStagesCount() => _gamePanelsModel.GetStagesCount();
        public int GetCurrentRoomLevel() => _gamePanelsModel.GetCurrentRoomLevel();
        public float GetAmbientVolume() => _gamePanelsModel.AmbientVolumeValue;
        public float GetSfxVolume() => _gamePanelsModel.SfxVolumeValue;
        public bool GetMuteStatus() => _gamePanelsModel.IsMuted;
        public void SetLanguage(string value) => _gamePanelsModel.SetLanguage(value);
        public void SetAmbientVolume(float volume) => _gamePanelsModel.SetAmbientVolume(volume);
        public void SetSfxVolume(float volume) => _gamePanelsModel.SetSfxVolume(volume);
        public void SetMuteStatus(bool value) => _gamePanelsModel.SetMute(value);
    }
}