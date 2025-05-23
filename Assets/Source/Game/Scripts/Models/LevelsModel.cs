using Assets.Source.Game.Scripts;
using IJunior.TypedScenes;
using System.Collections;
using UnityEngine;

public class LevelsModel
{
    private readonly TemporaryData _temporaryData;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly float _loadControlValue = 0.9f;
    private readonly GameObject _canvasLoader;

    private AsyncOperation _load;
    private LevelData _currentLevelData;
    private PlayerClassData _currentPlayerClassData;
    private WeaponData _currentWeaponData;

    public LevelsModel(TemporaryData temporaryData, ICoroutineRunner coroutineRunner, GameObject canvasLoader)
    {
        _temporaryData = temporaryData;
        _coroutineRunner = coroutineRunner;
        _canvasLoader = canvasLoader;
        _canvasLoader.gameObject.SetActive(false);
        LevelStates = _temporaryData.GetLevelStates();
    }

    public LevelState[] LevelStates { get; private set; }

    public bool TryUnlockContractButton(int index) 
    {
        LevelState levelState = _temporaryData.GetLevelState(index);

        if (levelState != null)
            return levelState.IsComplete;

        return false;
    }

    public bool TryBuyContract(int cost)
    {
        return _temporaryData.TrySpendCoins(cost);
    }

    public LevelState GetLevelState(LevelData levelData) 
    {
        LevelState levelState = _temporaryData.GetLevelState(levelData.Id);

        if (levelState == null)
            levelState = InitLevelState(levelData);

        return levelState;
    }

    public WeaponState GetWeaponState(WeaponData weaponData)
    {
        return _temporaryData.GetWeaponState(weaponData.Id);
    }

    public void SelectLevel(LevelDataView levelDataView)
    {
        _currentLevelData = levelDataView.LevelData;
        _temporaryData.SetLevelData(_currentLevelData);
    }

    public void SelectClass(PlayerClassDataView playerClassDataView) 
    {
        _currentPlayerClassData = playerClassDataView.PlayerClassData;
        _temporaryData.SetPlayerClassData(_currentPlayerClassData);
    }

    public void SelectWeapon(WeaponDataView weaponDataView) 
    {
        _currentWeaponData = weaponDataView.WeaponData;
        _temporaryData.SetWeaponData(_currentWeaponData);
    }

    public void LoadLevel() 
    {
        LoadScene(_currentLevelData.Id);
    }

    public int GetPlayerCoinCount() => _temporaryData.Coins;

    private void LoadScene(int id)
    {
        _coroutineRunner.StartCoroutine(LoadScreenLevel(GameScene.LoadAsync(_temporaryData)));
    }

    private IEnumerator LoadScreenLevel(AsyncOperation asyncOperation)
    {
        if (_load != null)
            yield break;

        _load = asyncOperation;
        _load.allowSceneActivation = false;
        _canvasLoader.gameObject.SetActive(true);

        while (_load.progress < _loadControlValue)
        {
            yield return null;
        }

        _load.allowSceneActivation = true;
        _load = null;
    }

    private LevelState InitLevelState(LevelData levelData)
    {
        LevelState levelState = new ()
        {
            Id = levelData.Id,
            IsComplete = false,
            CurrentCompleteStages = 0
        };

        return levelState;
    }
}