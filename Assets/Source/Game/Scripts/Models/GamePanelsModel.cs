using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class GamePanelsModel
    {
        private readonly TemporaryData _temporaryData;
        private readonly Player _player;
        private readonly LevelObserver _levelObserver;
        private readonly CardLoader _cardLoader;
        private readonly IAudioPlayerService _audioPlayerService;

        public GamePanelsModel(
            TemporaryData temporaryData,
            Player player,
            LevelObserver levelObserver,
            CardLoader cardLoader,
            IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            _temporaryData = temporaryData;
            _player = player;
            _levelObserver = levelObserver;
            _cardLoader = cardLoader;
            _levelObserver.StageCompleted += OnStageComplete;
            _cardLoader.CardPoolCreated += OnCardPoolCreate;
        }

        public event Action StageCompleted;
        public event Action CardPoolCreated;

        public string LanguageTag { get; private set; }
        public float AmbientVolumeValue { get; private set; }
        public float SfxVolumeValue { get; private set; }
        public bool IsMuted { get; private set; }

        public Player GetPlayer() 
        {
            return _player;
        }

        public List<CardData> GetMainCardPool() 
        {
            return _cardLoader.MainCardsPool;
        }

        public int GetStagesCount() 
        {
            return _levelObserver.CountStages;
        }

        public int GetCurrentRoomLevel() 
        {
            return _levelObserver.CurrentRoomLevel;
        }

        public void SetAmbientVolume(float volume)
        {
            AmbientVolumeValue = volume;
            _temporaryData.SetAmbientVolume(volume);
        }

        public void SetLanguage(string value)
        {
            LanguageTag = value;
            _temporaryData.SetCurrentLanguage(value);
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolumeValue = volume;
            _temporaryData.SetInterfaceVolume(volume);
        }

        public void CreateCardPool() 
        {
            _cardLoader.CreateCardPool();
        }

        public void SetMute(bool muted)
        {
            if (muted)
                Mute();
            else
                UnMute();
        }

        public IAudioPlayerService GetAudioService() 
        {
            return _audioPlayerService;
        }

        private void OnCardPoolCreate() 
        {
            CardPoolCreated?.Invoke();
        }

        private void OnStageComplete() 
        {
            StageCompleted?.Invoke();
        }

        private void Mute()
        {
            IsMuted = true;
            _temporaryData.SetMuteStateSound(IsMuted);
        }

        private void UnMute()
        {
            IsMuted = false;
            _temporaryData.SetMuteStateSound(IsMuted);
        }
    }
}