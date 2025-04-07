using System;
using System.Collections.Generic;

namespace Assets.Source.Game.Scripts
{
    public class GamePanelsModel
    {
        private readonly Random _rnd = new ();
        private readonly TemporaryData _temporaryData;
        private readonly Player _player;
        private readonly LevelObserver _levelObserver;
        private readonly CardLoader _cardLoader;
        private readonly int _minWeaponCount = 1;
        private readonly IAudioPlayerService _audioPlayerService;

        private List<WeaponData> _weaponDatas = new ();
        private WeaponData _rewardWeaponData;
        private WeaponState _rewardWeaponState;

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
            _levelObserver.GameEnded += OnGameEnded;
            _cardLoader.CardPoolCreated += OnCardPoolCreate;
        }

        public event Action StageCompleted;
        public event Action CardPoolCreated;
        public event Action GameEnded;

        public string LanguageTag { get; private set; }
        public float AmbientVolumeValue { get; private set; }
        public float SfxVolumeValue { get; private set; }
        public bool IsMuted { get; private set; }

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

        private void OnGameEnded()
        {
            GameEnded?.Invoke();
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

        private void CreateWeaponDatasForReward() 
        {
            for (int index = 0; index < (_temporaryData.LevelData as ContractLevelData).WeaponDatas.Length; index++) 
            {
                if (_temporaryData.GetWeaponState((_temporaryData.LevelData as ContractLevelData).WeaponDatas[index].Id) == null)
                    _weaponDatas[index] = (_temporaryData.LevelData as ContractLevelData).WeaponDatas[index];
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