using Assets.Source.Game.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopService : IDisposable
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly ISaveAndLoadProgress _saveAndLoadProgress;
    private readonly GamePanelsService _gamePanelsService;
    private readonly RoomService _roomService;
    private readonly PersistentDataService _persistentDataService;
    private readonly Player _player;
    private readonly GameObject _canvasLoader;
    private readonly float _loadControlValue = 0.9f;
    private readonly string _menuSceneName = "Menu";

    private AsyncOperation _load;

    public event Action GameClosed;

    public GameLoopService(
        ICoroutineRunner coroutineRunner,
        ISaveAndLoadProgress saveAndLoadProgress,
        GamePanelsService gamePanelsService,
        RoomService roomService,
        PersistentDataService persistentDataService,
        Player player,
        GameObject canvasLoader) 
    {
        _coroutineRunner = coroutineRunner;
        _saveAndLoadProgress = saveAndLoadProgress;
        _gamePanelsService = gamePanelsService;
        _roomService = roomService;
        _persistentDataService = persistentDataService;
        _player = player;
        _canvasLoader = canvasLoader;
        _gamePanelsService.GameClosed += OnGameClosed;
    }

    public void Dispose()
    {
        _gamePanelsService.GameClosed -= OnGameClosed;
        GC.SuppressFinalize(this);
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
