#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class BootSceneSetupTool
{
    private const string SOPath = "Assets/ScriptableObjects";

    [MenuItem("NumbersBlast/Setup Boot Scene")]
    private static void SetupBootScene()
    {
        var audioConfig = CreateOrLoadAudioConfig();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var cameraGo = new GameObject("Main Camera");
        var camera = cameraGo.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        cameraGo.tag = "MainCamera";

        // ProjectLifetimeScope
        var scopeGo = new GameObject("ProjectLifetimeScope");
        var scope = scopeGo.AddComponent<ProjectLifetimeScope>();

        var so = new SerializedObject(scope);
        so.FindProperty("_audioConfig").objectReferenceValue = audioConfig;
        so.ApplyModifiedPropertiesWithoutUndo();

        // BootLoader
        var bootGo = new GameObject("BootLoader");
        bootGo.AddComponent<BootLoader>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/BootScene.unity");
        Debug.Log("[BootSceneSetup] Boot scene created.");
    }

    [MenuItem("NumbersBlast/Setup All Scenes")]
    private static void SetupAllScenes()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new("Assets/Scenes/BootScene.unity", true),
            new("Assets/Scenes/MainMenuScene.unity", true),
            new("Assets/Scenes/GameScene.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("[BootSceneSetup] Build settings updated: Boot(0) -> MainMenu(1) -> Game(2)");
    }

    private static AudioConfig CreateOrLoadAudioConfig()
    {
        var path = SOPath + "/AudioConfig.asset";
        var existing = AssetDatabase.LoadAssetAtPath<AudioConfig>(path);
        if (existing != null) return existing;

        if (!AssetDatabase.IsValidFolder(SOPath))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");

        var config = ScriptableObject.CreateInstance<AudioConfig>();
        AssetDatabase.CreateAsset(config, path);
        return config;
    }
}
#endif
