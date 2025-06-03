using Assets.Source.Game.Scripts;
using UnityEngine;

public class GameSceneLoader : SceneLoader
{
    [SerializeField] private LevelObserver _levelObserver;

    public override void OnSceneLoaded(TemporaryData temporaryData)
    {
        //Сделать префаб Scripts.
        _levelObserver.Initialize(temporaryData);
    }
}