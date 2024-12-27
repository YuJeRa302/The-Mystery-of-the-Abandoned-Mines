namespace IJunior.TypedScenes
{
    using UnityEngine.SceneManagement;
    
    public class GameScene : TypedScene
    {
        public const int Id = 0;

        private const string _sceneName = "GameScene";

        public static void Load(TemporaryData temporaryData, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            LoadScene(_sceneName, loadSceneMode, temporaryData);
        }

        public static UnityEngine.AsyncOperation LoadAsync(TemporaryData temporaryData, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            return LoadScene(_sceneName, loadSceneMode, temporaryData);
        }
    }
}
