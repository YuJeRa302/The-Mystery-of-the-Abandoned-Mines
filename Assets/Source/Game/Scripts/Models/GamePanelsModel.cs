using Lean.Localization;
using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class GamePanelsModel
    {
        private readonly int _countRerollPointsReward = 2;
        private readonly System.Random _rnd = new ();
        private readonly TemporaryData _temporaryData;
        private readonly Player _player;
        private readonly LevelObserver _levelObserver;
        private readonly CardLoader _cardLoader;
        private readonly int _minWeaponCount = 1;
        private readonly AudioPlayer _audioPlayer;
        private readonly LeanLocalization _leanLocalization;

        private List<WeaponData> _weaponDatas = new ();
        private WeaponData _rewardWeaponData;
        private WeaponState _rewardWeaponState;

        public GamePanelsModel(
            TemporaryData temporaryData,
            Player player,
            LevelObserver levelObserver,
            CardLoader cardLoader,
            AudioPlayer audioPlayer, LeanLocalization leanLocalization)
        {
            _audioPlayer = audioPlayer;
            _temporaryData = temporaryData;
            _player = player;
            _levelObserver = levelObserver;
            _cardLoader = cardLoader;
            _leanLocalization = leanLocalization;
            AmbientVolumeValue = _temporaryData.AmbientVolume;
            SfxVolumeValue = _temporaryData.InterfaceVolume;
            _audioPlayer.AmbientValueChanged(AmbientVolumeValue);
            _audioPlayer.SfxValueChanged(SfxVolumeValue);
            _audioPlayer.PlayAmbient();
            IsMuted = _temporaryData.MuteStateSound;
            AddListeners();
        }

        public event Action StageCompleted;
        public event Action CardPoolCreated;
        public event Action<int> LootRoomComplitetd;
        public event Action CardPanelOpened;
        public event Action<bool> GameEnded;

        public string LanguageTag { get; private set; }
        public float AmbientVolumeValue { get; private set; }
        public float SfxVolumeValue { get; private set; }
        public bool IsMuted { get; private set; } = false;

        public void OpenCardPanel() 
        {
            CardPanelOpened?.Invoke();
        }

        public bool GetLevelType() 
        {
            return _temporaryData.LevelData.IsContractLevel;
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
            return _levelObserver.CountStages;
        }

        public int GetCurrentRoomLevel() 
        {
            return _levelObserver.CurrentRoomLevel;
        }

        public WeaponData CreateRewardWeapon()
        {
            CreateWeaponDatasForReward();
            GetRandomRewardWeapon();
            _rewardWeaponState = CreateStateForRewardWeapon(_rewardWeaponData);
            _temporaryData.UpdateWeaponStates(_rewardWeaponState);
            return _rewardWeaponData;
        }

        public void SetAmbientVolume(float volume)
        {
            AmbientVolumeValue = volume;
            _audioPlayer.AmbientValueChanged(AmbientVolumeValue);
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
            _audioPlayer.SfxValueChanged(SfxVolumeValue);
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

        public void GetLootRoomReward(int reward)
        {
            _player.GetLootRoomReward(reward);
        }

        public void Mute()
        {
            IsMuted = true;
            _temporaryData.SetMuteStateSound(IsMuted);
            _audioPlayer.MuteSound(IsMuted);
        }

        public void UnMute()
        {
            IsMuted = false;
            _temporaryData.SetMuteStateSound(IsMuted);
            _audioPlayer.MuteSound(IsMuted);
        }

        private void AddListeners() 
        {
            _levelObserver.StageCompleted += OnStageComplete;
            _levelObserver.GameEnded += OnGameEnded;
            _levelObserver.LootRoomComplited += OnLootRoomComplited;
            _levelObserver.GamePaused += OnGamePause;
            _levelObserver.GameResumed += OnGameResume;
            _cardLoader.CardPoolCreated += OnCardPoolCreate;
        }

        private void RemoveListeners() 
        {
            _levelObserver.StageCompleted -= OnStageComplete;
            _levelObserver.GameEnded -= OnGameEnded;
            _levelObserver.LootRoomComplited -= OnLootRoomComplited;
            _levelObserver.GamePaused -= OnGamePause;
            _levelObserver.GameResumed -= OnGameResume;
            _cardLoader.CardPoolCreated -= OnCardPoolCreate;
        }

        private void OnLootRoomComplited(int reward)
        {
            LootRoomComplitetd?.Invoke(reward);
        }

        private void OnCardPoolCreate() 
        {
            CardPoolCreated?.Invoke();
        }

        private void OnStageComplete() 
        {
            StageCompleted?.Invoke();
        }

        private void OnGameEnded(bool state)
        {
            _audioPlayer.StopAmbient();
            GameEnded?.Invoke(state);
        }

        private void OnGamePause(bool state)
        {
            _audioPlayer.MuteSound(state);
        }

        private void OnGameResume(bool state)
        {
            _audioPlayer.MuteSound(_temporaryData.MuteStateSound);
        }

        private void CreateWeaponDatasForReward() 
        {
            for (int index = 0; index < (_temporaryData.LevelData as ContractLevelData).WeaponDatas.Length; index++) 
            {
                if (_temporaryData.GetWeaponState((_temporaryData.LevelData as ContractLevelData).WeaponDatas[index].Id) == null)
                    _weaponDatas.Add((_temporaryData.LevelData as ContractLevelData).WeaponDatas[index]);
            }
        }

        private void GetRandomRewardWeapon() 
        {
            _rewardWeaponData = null;

            if (_weaponDatas.Count > 0)
            {
                if (_weaponDatas.Count > _minWeaponCount)
                    _rewardWeaponData = _weaponDatas[_rnd.Next(0, _weaponDatas.Count)];
                else
                    _rewardWeaponData = _weaponDatas[0];
            }
        }

        private WeaponState CreateStateForRewardWeapon(WeaponData weaponData) 
        {
            if (weaponData == null)
                return null;

            WeaponState weaponState = new ();
            weaponState.Id = weaponData.Id;
            weaponState.IsEquip = false;
            weaponState.IsUnlock = true;
            return weaponState;
        }
    }
}