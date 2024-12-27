using IJunior.TypedScenes;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class SceneLoader : MonoBehaviour, ISceneLoadHandler<TemporaryData>
    {
        [SerializeField] private LevelObserver _levelObserver;

        public void OnSceneLoaded(TemporaryData temporaryData)
        {
            _levelObserver.Initialize(temporaryData);
        }
    }
}
