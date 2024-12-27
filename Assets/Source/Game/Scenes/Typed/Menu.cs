namespace IJunior.TypedScenes
{
    using UnityEngine.SceneManagement;
    
    public class Menu : TypedScene
    {
        
        private const string _sceneName = "Menu";

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
