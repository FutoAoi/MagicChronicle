using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// InitSceneのブートローダー。
/// 常駐オブジェクト（CriAtom / CriAudioManager等）が確実に初期化されてから
/// 次のシーンへ遷移させます。
///
/// エディタ実行時、EditorAutoBootstrapによって自動的にInitScene経由で
/// 再生された場合は、元々開いていたシーンへ自動的に遷移します。
/// （実機ビルド・通常のゲーム起動時は常にNextSceneNameへ遷移します）
/// </summary>
public class InitSceneBootstrap : MonoBehaviour
{
    [Header("遷移先シーン（通常起動時）")]
    [Tooltip("初期化完了後にロードするシーン名（Build Settingsに登録済みであること）")]
    [SerializeField] private string _nextSceneName = "Title";

    [Header("常駐オブジェクトルート")]
    [Tooltip("DontDestroyOnLoadを適用する対象。未設定ならこのGameObjectの親を自動探索します")]
    [SerializeField] private GameObject _systemsRoot;

    [Header("デバッグ")]
    [SerializeField] private bool _enableDebugLog = true;

    private const string SESSION_KEY_ORIGINAL_SCENE = "EditorAutoBootstrap_OriginalScenePath";

    private IEnumerator Start()
    {
        if (_enableDebugLog)
            Debug.Log("[InitSceneBootstrap] 初期化開始");

        if (_systemsRoot != null)
        {
            DontDestroyOnLoad(_systemsRoot);
        }

        // CriAudioManagerの初期化完了を待つ
        yield return new WaitUntil(() => CriAudioManager.Instance != null);

        if (_enableDebugLog)
            Debug.Log("[InitSceneBootstrap] CriAudioManager 初期化確認OK。遷移先を決定します。");

        yield return null;

        string targetSceneName = ResolveTargetSceneName();

        if (_enableDebugLog)
            Debug.Log($"[InitSceneBootstrap] シーン遷移: {targetSceneName}");

        SceneManager.LoadScene(targetSceneName);
    }

    /// <summary>
    /// 遷移先シーン名を決定します。
    /// エディタでEditorAutoBootstrapにより起動された場合は元のシーンへ、
    /// それ以外（実機・通常起動）は_nextSceneNameへ遷移します。
    /// </summary>
    private string ResolveTargetSceneName()
    {
#if UNITY_EDITOR
        string originalScenePath = SessionState.GetString(SESSION_KEY_ORIGINAL_SCENE, string.Empty);

        if (!string.IsNullOrEmpty(originalScenePath))
        {
            // 使用後は消しておく（次回の意図しない再利用を防ぐ）
            SessionState.EraseString(SESSION_KEY_ORIGINAL_SCENE);

            string sceneName = System.IO.Path.GetFileNameWithoutExtension(originalScenePath);

            if (_enableDebugLog)
                Debug.Log($"[InitSceneBootstrap] エディタ自動起動を検知。元のシーンへ遷移します: {sceneName}");

            return sceneName;
        }
#endif
        return _nextSceneName;
    }
}