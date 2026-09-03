#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// エディタで任意のシーンから直接再生した場合でも、
/// 必ずInitSceneを経由してCriAudioManager等の常駐オブジェクトを
/// 初期化してから、元々開いていたシーンへ自動遷移させるための仕組み。
///
/// 仕組み:
/// 1. 再生ボタンを押した瞬間(ExitingEditMode)に、現在開いているシーンのパスを
///    SessionStateに保存し、playModeStartSceneをInitSceneに差し替える。
/// 2. Unity標準機能により、再生開始時にInitSceneが最初にロードされる。
/// 3. InitScene側のInitSceneBootstrap（別ファイル）が、SessionStateに保存された
///    シーンへ初期化完了後に自動遷移する。
/// 4. 再生を停止した瞬間(EnteredEditMode)に、playModeStartSceneをnullへ戻し、
///    通常の編集作業に影響が出ないようにする。
///
/// 前提: InitSceneはBuild Settingsに登録されており、
///       INIT_SCENE_PATH の値と実際のファイルパスが一致していること。
/// </summary>
[InitializeOnLoad]
public static class EditorAutoBootstrap
{
    // InitSceneの実際のアセットパスに合わせて変更してください
    private const string INIT_SCENE_PATH = "Assets/_scenes/InitScene.unity";

    // SessionStateはドメインリロードを跨いでも値が保持されるキー・バリューストア
    private const string SESSION_KEY_ORIGINAL_SCENE = "EditorAutoBootstrap_OriginalScenePath";

    // このEditor拡張自体のON/OFFを切り替えたい場合用（メニューから変更可能）
    private const string MENU_PATH = "Tools/Auto Bootstrap/Enable InitScene Auto Boot";
    private const string PREFS_KEY_ENABLED = "EditorAutoBootstrap_Enabled";

    static EditorAutoBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static bool IsEnabled
    {
        get => EditorPrefs.GetBool(PREFS_KEY_ENABLED, true);
        set => EditorPrefs.SetBool(PREFS_KEY_ENABLED, value);
    }

    [MenuItem(MENU_PATH)]
    private static void ToggleEnabled()
    {
        IsEnabled = !IsEnabled;
    }

    [MenuItem(MENU_PATH, true)]
    private static bool ToggleEnabledValidate()
    {
        Menu.SetChecked(MENU_PATH, IsEnabled);
        return true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (!IsEnabled) return;

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            HandleExitingEditMode();
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            HandleEnteredEditMode();
        }
    }

    /// <summary>再生ボタンを押した瞬間の処理。</summary>
    private static void HandleExitingEditMode()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        string currentScenePath = activeScene.path;

        // 保存されていないシーン（未保存の新規シーン等）の場合は何もしない
        if (string.IsNullOrEmpty(currentScenePath))
        {
            Debug.LogWarning("[EditorAutoBootstrap] 現在のシーンが未保存のため、AutoBootstrapをスキップします。");
            return;
        }

        // 既にInitSceneを開いている場合は差し替え不要（そのまま通常再生させる）
        if (currentScenePath == INIT_SCENE_PATH)
        {
            SessionState.EraseString(SESSION_KEY_ORIGINAL_SCENE);
            return;
        }

        SceneAsset initSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(INIT_SCENE_PATH);
        if (initSceneAsset == null)
        {
            Debug.LogWarning($"[EditorAutoBootstrap] InitSceneが見つかりません: {INIT_SCENE_PATH}\n" +
                              "パスを確認するか、EditorAutoBootstrap.cs内のINIT_SCENE_PATHを修正してください。");
            return;
        }

        // 元のシーンパスを記憶しておく（InitScene側で読み出して遷移先にする）
        SessionState.SetString(SESSION_KEY_ORIGINAL_SCENE, currentScenePath);

        // 再生開始時にInitSceneを最初にロードするよう設定
        EditorSceneManager.playModeStartScene = initSceneAsset;

        Debug.Log($"[EditorAutoBootstrap] {currentScenePath} から再生開始 → InitScene経由に切り替えます。");
    }

    /// <summary>再生を停止した瞬間の処理。</summary>
    private static void HandleEnteredEditMode()
    {
        // 通常の編集作業に影響しないよう、必ず元に戻す
        EditorSceneManager.playModeStartScene = null;
    }
}
#endif