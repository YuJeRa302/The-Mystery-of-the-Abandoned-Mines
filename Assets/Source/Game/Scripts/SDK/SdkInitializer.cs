using System.Collections;
using YG;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Game.Scripts
{
    public class SdkInitializer : MonoBehaviour
    {
        private readonly string _firstScene = "Menu";

        private IEnumerator Start()
        {
            if (YandexGame.SDKEnabled)
            {
                yield return null;
                Initialize();
            }
        }

        private void Initialize()
        {
            SceneManager.LoadScene(_firstScene);
        }
    }
}