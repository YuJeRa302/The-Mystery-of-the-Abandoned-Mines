using System.Collections;
using YG;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts
{
    public class SdkInitializer : MonoBehaviour
    {
        private readonly float _loadControlValue = 0.9f;
        private readonly float _delayLoadScene = 0.8f;
        private readonly string _firstScene = "Menu";

        [SerializeField] private GameObject _canvasLoader;

        private AsyncOperation _load;

        private void OnEnable() => YandexGame.GetDataEvent += OnInitialize;

        private void OnDisable() => YandexGame.GetDataEvent -= OnInitialize;

        private void OnInitialize()
        {
            if (YandexGame.SDKEnabled == true)
                StartCoroutine(LoadScreenLevel(SceneManager.LoadSceneAsync(_firstScene)));
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
                yield return new WaitForSeconds(_delayLoadScene);
            }

            _load.allowSceneActivation = true;
            _load = null;
        }
    }
}