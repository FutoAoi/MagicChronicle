#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EditorAutoBootstrap
{
    static EditorAutoBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // 現在のシーンを記憶し、InitSceneが再生開始シーンになるようにする
            // （必要に応じてEditorSceneManager.playModeStartSceneを使う）
        }
    }
}
#endif