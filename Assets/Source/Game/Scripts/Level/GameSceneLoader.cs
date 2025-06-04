using Assets.Source.Game.Scripts;
using UnityEngine;

public class GameSceneLoader : SceneLoader
{
    [SerializeField] private LevelObserver _levelObserver;

    public override void OnSceneLoaded(TemporaryData temporaryData)
    {
        _levelObserver.Initialize(temporaryData);
    }
}