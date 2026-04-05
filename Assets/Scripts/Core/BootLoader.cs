using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    private const int TargetFrameRateMobile = 60;
    private const int TargetFrameRateEditor = 120;
    private const int VSyncOff = 0;

    private void Awake()
    {
        ConfigureApplication();
    }

    private void Start()
    {
        SceneManager.LoadScene(GameConstants.MainMenuScene);
    }

    private void ConfigureApplication()
    {
        QualitySettings.vSyncCount = VSyncOff;

#if UNITY_EDITOR
        Application.targetFrameRate = TargetFrameRateEditor;
#elif UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = TargetFrameRateMobile;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#else
        Application.targetFrameRate = TargetFrameRateMobile;
#endif
    }
}
