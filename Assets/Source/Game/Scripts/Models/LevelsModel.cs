using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts
{
    public class LevelsModel
    {
        private readonly PersistentDataService _persistentDataService;
        private readonly GameObject _canvasLoader;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ISaveAndLoadProgress _saveAndLoadProgress;
        private readonly float _loadControlValue = 0.9f;
        private readonly string _gameSceneName = "GameScene";

        private AsyncOperation _load;

        public LevelsModel(
            PersistentDataService persistentDataService,
            ICoroutineRunner coroutineRunner,
            ISaveAndLoadProgress saveAndLoadProgress,
            GameObject canvasLoader)
        {
            _coroutineRunner = coroutineRunner;
            _saveAndLoadProgress = saveAndLoadProgress;
            _persistentDataService = persistentDataService;
            _canvasLoader = canvasLoader;
            _canvasLoader.gameObject.SetActive(false);
        }

        public bool TryUnlockContractButton(int index)
        {
            LevelState levelState = _persistentDataService.PlayerProgress.LevelService.GetLevelStateById(index);

            if (levelState != null)
                return levelState.IsComplete;

            return false;
        }

        public bool TryBuyContract(int cost)
        {
            return _persistentDataService.TrySpendCoins(cost);
        }

        public bool TryUnlockLevel(int levelId)
        {
            return _persistentDataService.PlayerProgress.LevelService.GetLevelStateById(levelId).IsComplete;
        }

        public LevelState GetLevelState(LevelData levelData)
        {
            return _persistentDataService.PlayerProgress.LevelService.GetLevelStateByLevelData(levelData);
        }

        public WeaponState GetWeaponState(WeaponData weaponData)
        {
            return _persistentDataService.PlayerProgress.WeaponService.GetWeaponStateByData(weaponData);
        }

        public int GetPlayerCoinCount() => _persistentDataService.PlayerProgress.Coins;

        public void SelectLevel(LevelDataView levelDataView)
        {
            _persistentDataService.PlayerProgress.LevelService.CurrentLevelId = levelDataView.LevelData.Id;
        }

        public void SelectClass(PlayerClassDataView playerClassDataView)
        {
            _persistentDataService.PlayerProgress.CurrentPlayerClassId = playerClassDataView.PlayerClassData.Id;
        }

        public void SelectWeapon(WeaponDataView weaponDataView)
        {
            _persistentDataService.PlayerProgress.WeaponService.CurrentWeaponId = weaponDataView.WeaponData.Id;
        }

        public void LoadScene()
        {
            _saveAndLoadProgress.SaveDataToPrefs();
            _coroutineRunner.StartCoroutine(LoadScreenLevel(SceneManager.LoadSceneAsync(_gameSceneName)));
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