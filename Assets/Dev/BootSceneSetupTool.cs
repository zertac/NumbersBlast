#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class BootSceneSetupTool
{
    [MenuItem("NumbersBlast/Setup Boot Scene")]
    private static void SetupBootScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var cameraGo = new GameObject("Main Camera");
        var camera = cameraGo.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        cameraGo.tag = "MainCamera";

        // ProjectLifetimeScope
        var scopeGo = new GameObject("ProjectLifetimeScope");
        scopeGo.AddComponent<ProjectLifetimeScope>();

        // BootLoader
        var bootGo = new GameObject("BootLoader");
        bootGo.AddComponent<BootLoader>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/BootScene.unity");
        Debug.Log("[BootSceneSetup] Boot scene created. Set as scene index 0 in Build Settings.");
    }

    [MenuItem("NumbersBlast/Setup All Scenes")]
    private static void SetupAllScenes()
    {
        // Add scenes to build settings
        var scenes = new EditorBuildSettingsScene[]
        {
            new("Assets/Scenes/BootScene.unity", true),
            new("Assets/Scenes/MainMenuScene.unity", true),
            new("Assets/Scenes/GameScene.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("[BootSceneSetup] Build settings updated: Boot(0) -> MainMenu(1) -> Game(2)");
    }
}
#endif
