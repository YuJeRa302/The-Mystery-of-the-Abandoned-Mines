using Lean.Localization;
using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class GamePanelsModel
    {
        private readonly string _defalutRewardIndex = "0";
        private readonly string _rerollPointsRewardIndex = "1";
        private readonly int _countRerollPointsReward = 2;
        private readonly int _minWeaponCount = 1;
        private readonly System.Random _rnd = new ();
        private readonly PersistentDataService _persistentDataService;
        private readonly RoomService _roomService;
        private readonly GamePauseService _gamePauseService;
        private readonly Player _player;
        private readonly CardLoader _cardLoader;
        private readonly AudioPlayer _audioPlayer;
        private readonly LeanLocalization _leanLocalization;
        private readonly LevelData _currentLevelData;

        private List<WeaponData> _weaponDatasForReward = new ();
        private WeaponData _rewardWeaponData;

        public GamePanelsModel(
            RoomService roomService,
            GamePauseService gamePauseService,
            PersistentDataService persistentDataService,
            CardLoader cardLoader,
            Player player,
            LevelData levelData,
            AudioPlayer audioPlayer,
            LeanLocalization leanLocalization) 
        {
            _roomService = roomService;
            _gamePauseService = gamePauseService;
            _persistentDataService = persistentDataService;
            _cardLoader = cardLoader;
            _player = player;
            _currentLevelData = levelData;
            _audioPlayer = audioPlayer;
            _leanLocalization = leanLocalization;
            SetApplicationParameters();
            AddListeners();
        }

        public event Action StageCompleted;
        public event Action CardPoolCreated;
        public event Action<int> LootRoomCompleted;
        public event Action CardPanelOpened;
        public event Action<bool> GameEnded;

        public float AmbientVolumeValue { get; private set; }
        public float SfxVolumeValue { get; private set; }
        public bool IsMuted { get; private set; } = false;
        public AudioPlayer AudioPlayer => _audioPlayer;

        public string GetRerollPointsRewardIndex() 
        {
            return _rerollPointsRewardIndex;
        }

        public string GetDefalutRewardIndex()
        {
            return _defalutRewardIndex;
        }

        public bool GetLevelType()
        {
            return _currentLevelData.IsContractLevel;
        }

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
            return _currentLevelData.CountStages;
        }

        public int GetCurrentRoomLevel()
        {
            return _roomService.CurrentRoomLevel;
        }

        public WeaponData CreateRewardWeapon()
        {
            CreateWeaponDatasForReward();
            GetRandomRewardWeapon();
            _persistentDataService.PlayerProgress.WeaponService.UnlockWeaponByData(_rewardWeaponData);
            return _rewardWeaponData;
        }

        public void SetAmbientVolume(float volume)
        {
            AmbientVolumeValue = volume;
            _audioPlayer.AmbientValueChanged(AmbientVolumeValue);
            _persistentDataService.PlayerProgress.AmbientVolume = AmbientVolumeValue;
        }

        public void SetLanguage(string value)
        {
            _persistentDataService.PlayerProgress.Language = value;
            _leanLocalization.SetCurrentLanguage(value);
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolumeValue = volume;
            _audioPlayer.SfxValueChanged(SfxVolumeValue);
            _persistentDataService.PlayerProgress.SfxVolume = SfxVolumeValue;
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
            return _audioPlayer;
        }

        public void GetRerollPointsReward()
        {
            _player.GetRerollPointsReward(_countRerollPointsReward);
        }

        public void GetEndGameReward()
        {
            _player.GetEndGameReward();
        }

        public void Mute()
        {
            IsMuted = true;
            _persistentDataService.PlayerProgress.IsMuted = IsMuted;
            _audioPlayer.MuteSound(IsMuted);
        }

        public void UnMute()
        {
            IsMuted = false;
            _persistentDataService.PlayerProgress.IsMuted = IsMuted;
            _audioPlayer.MuteSound(IsMuted);
        }

        private void AddListeners()
        {
            _roomService.StageCompleted += OnStageComplete;
            _roomService.LootRoomCompleted += OnLootRoomComplited;
            _roomService.GameEnded += OnGameEnded;
            _gamePauseService.GamePaused += OnGamePause;
            _gamePauseService.GameResumed += OnGameResume;
            _cardLoader.CardPoolCreated += OnCardPoolCreate;
            _player.PlayerLevelChanged += OnPlayerLevelChanged;
        }

        private void OnPlayerLevelChanged()
        {
            CardPanelOpened?.Invoke();
        }

        private void OnGameEnded(bool state)
        {
            _audioPlayer.StopAmbient();
            GameEnded?.Invoke(state);
        }

        private void OnLootRoomComplited(int reward)
        {
            LootRoomCompleted?.Invoke(reward);
        }

        private void OnCardPoolCreate()
        {
            CardPoolCreated?.Invoke();
        }

        private void OnStageComplete()
        {
            StageCompleted?.Invoke();
        }

        private void OnGamePause(bool state)
        {
            if (_audioPlayer != null)
                _audioPlayer.MuteSound(state);

            _persistentDataService.PlayerProgress.IsMuted = IsMuted;
        }

        private void OnGameResume(bool state)
        {
            if (_audioPlayer != null)
                _audioPlayer.MuteSound(_persistentDataService.PlayerProgress.IsMuted);

            _persistentDataService.PlayerProgress.IsMuted = IsMuted;
        }

        private void SetApplicationParameters() 
        {
            IsMuted = _persistentDataService.PlayerProgress.IsMuted;
            AmbientVolumeValue = _persistentDataService.PlayerProgress.AmbientVolume;
            SfxVolumeValue = _persistentDataService.PlayerProgress.SfxVolume;
            _audioPlayer.AmbientValueChanged(AmbientVolumeValue);
            _audioPlayer.SfxValueChanged(SfxVolumeValue);
            _audioPlayer.PlayAmbient();
            _audioPlayer.MuteSound(IsMuted);
            SetLanguage(_persistentDataService.PlayerProgress.Language);
        }

        private void CreateWeaponDatasForReward()
        {
            for (int index = 0; index < (_currentLevelData as ContractLevelData).WeaponDatas.Length; index++)
            {
                if (_persistentDataService.PlayerProgress.WeaponService.GetWeaponStateByData(
                    (_currentLevelData as ContractLevelData).WeaponDatas[index]).IsUnlock == false)
                    _weaponDatasForReward.Add((_currentLevelData as ContractLevelData).WeaponDatas[index]);
            }
        }

        private void GetRandomRewardWeapon()
        {
            _rewardWeaponData = null;

            if (_weaponDatasForReward.Count > 0)
            {
                if (_weaponDatasForReward.Count > _minWeaponCount)
                    _rewardWeaponData = _weaponDatasForReward[_rnd.Next(0, _weaponDatasForReward.Count)];
                else
                    _rewardWeaponData = _weaponDatasForReward[0];
            }
        }
    }
}