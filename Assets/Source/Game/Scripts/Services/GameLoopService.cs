using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.GamePanels;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.Services
{
    public class GameLoopService : IDisposable
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ISaveAndLoadProgress _saveAndLoadProgress;
        private readonly PersistentDataService _persistentDataService;
        private readonly GameObject _canvasLoader;
        private readonly float _loadControlValue = 0.9f;
        private readonly string _menuSceneName = "Menu";

        private CompositeDisposable _disposables = new ();
        private RoomService _roomService;
        private Player _player;
        private AsyncOperation _load;

        public GameLoopService(
            ICoroutineRunner coroutineRunner,
            ISaveAndLoadProgress saveAndLoadProgress,
            PersistentDataService persistentDataService,
            GameObject canvasLoader)
        {
            _coroutineRunner = coroutineRunner;
            _saveAndLoadProgress = saveAndLoadProgress;
            _persistentDataService = persistentDataService;
            _canvasLoader = canvasLoader;
            MessageBroker.Default.Receive<M_CloseGame>().Subscribe(m => OnGameClosed()).AddTo(_disposables);
        }

        public event Action GameClosed;

        public void Dispose()
        {
            if (_disposables != null)
                _disposables.Dispose();
        }

        public void InitGameEntities(Player player, RoomService roomService)
        {
            _player = player;
            _roomService = roomService;
        }

        private void OnGameClosed()
        {
            _saveAndLoadProgress.SaveGameProgerss(
                _player.Score,
                _player.Coins,
                _player.UpgradePoints,
                _persistentDataService.PlayerProgress.LevelService.CurrentLevelId,
                _roomService.IsWinGame,
                _roomService.IsGameInterrupted);

            _coroutineRunner.StartCoroutine(LoadScreenLevel(SceneManager.LoadSceneAsync(_menuSceneName)));
            GameClosed?.Invoke();
        }

        private IEnumerator LoadScreenLevel(AsyncOperation asyncOperation)
        {
            if (_load != null)
                yield break;

            _canvasLoader.gameObject.SetActive(true);
            _load = asyncOperation;
            _load.allowSceneActivation = false;

            while (_load.progress < _loadControlValue)
            {
                yield return null;
            }

            _load.allowSceneActivation = true;
            _load = null;
        }
    }
}