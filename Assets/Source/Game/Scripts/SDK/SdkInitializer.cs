using System.Collections;
using YG;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts.SDK
{
    public class SdkInitializer : MonoBehaviour
    {
        private readonly float _loadControlValue = 0.9f;
        private readonly string _firstScene = "Menu";

        [SerializeField] private GameObject _canvasLoader;

        private AsyncOperation _load;

        private void OnEnable() => YG2.onGetSDKData += OnInitialize;

        private void OnDisable() => YG2.onGetSDKData -= OnInitialize;

        private void OnInitialize()
        {
            if (YG2.isSDKEnabled == true)
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
                yield return null;
            }

            _load.allowSceneActivation = true;
            _load = null;
        }
    }
}