using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitButton : MonoBehaviour
{
    /// <summary>
    /// ボタンのOnClickに登録する
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        // エディター上ではプレイモードを終了
        EditorApplication.isPlaying = false;
#else
        // ビルド後はアプリケーションを終了
        Application.Quit();
#endif
    }
}