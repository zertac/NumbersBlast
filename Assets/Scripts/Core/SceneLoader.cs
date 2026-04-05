using UnityEngine.SceneManagement;

namespace NumbersBlast.Core
{
    public class SceneLoader
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadGameplay()
        {
            LoadScene(GameConstants.GameScene);
        }

        public void LoadMainMenu()
        {
            LoadScene(GameConstants.MainMenuScene);
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
