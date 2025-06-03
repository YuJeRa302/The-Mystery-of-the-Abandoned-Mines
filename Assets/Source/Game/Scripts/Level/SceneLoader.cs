using IJunior.TypedScenes;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public abstract class SceneLoader : MonoBehaviour, ISceneLoadHandler<TemporaryData>
    {
        public abstract void OnSceneLoaded(TemporaryData temporaryData);
    }
}