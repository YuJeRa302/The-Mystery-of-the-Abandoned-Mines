using Assets.Source.Game.Scripts;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class CLEARSAVE : MonoBehaviour
{
    private const string DataKeyLocal = "PlayerDataLocalTest";

    [SerializeField] private MainMenuBuilder main;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        main.EnebleSave += Sum;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(Clear);
    }

    private void Sum()
    {
        button.onClick.AddListener(Clear);
    }

    private void Clear()
    {
        if (UnityEngine.PlayerPrefs.HasKey(DataKeyLocal))
        {
            UnityEngine.PlayerPrefs.DeleteKey(DataKeyLocal);
        }

        //YandexGame.ResetSaveProgress();
        //YandexGame.SaveProgress();
    }
}