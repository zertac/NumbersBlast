using UnityEngine.SceneManagement;

public class SceneLoader
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameplay()
    {
        LoadScene("GameScene");
    }

    public void LoadMainMenu()
    {
        LoadScene("MainMenuScene");
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
