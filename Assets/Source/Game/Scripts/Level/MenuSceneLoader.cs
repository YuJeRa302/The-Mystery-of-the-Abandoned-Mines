using Assets.Source.Game.Scripts;
using UnityEngine;

public class MenuSceneLoader : SceneLoader
{
    [SerializeField] private MainMenuBuilder _mainMenuBuilder;

    public override void OnSceneLoaded(TemporaryData temporaryData)
    {
        //_mainMenuBuilder.Initialize(temporaryData);
    }
}