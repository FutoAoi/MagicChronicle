using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 複数のパネル(GameObject)を左右の矢印ボタンでOn/Off切り替えする。
/// 演出なし、押した瞬間に即切り替え。
///
/// 使い方:
/// 1. このスクリプトを空のGameObject(例: "PanelSwitcher")にアタッチ
/// 2. Panels に切り替えたいパネルのGameObjectをInspectorから順番に登録
/// 3. PrevButton / NextButton に矢印ボタン(Button)をアサイン
/// </summary>
public class PanelSwitcher : MonoBehaviour
{
    [Tooltip("切り替え対象のパネル。表示したい順番に並べる")]
    [SerializeField] private GameObject[] panels;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [Tooltip("開始時に表示するパネルのインデックス")]
    [SerializeField] private int startIndex = 0;

    public int CurrentIndex;

    private void Awake()
    {
        if (prevButton != null) prevButton.onClick.AddListener(ShowPrev);
        if (nextButton != null) nextButton.onClick.AddListener(ShowNext);
    }

    private void Start()
    {
        CurrentIndex = Mathf.Clamp(startIndex, 0, panels.Length - 1);
        ShowPanel(CurrentIndex);
    }

    public void ShowNext()
    {
        if (panels.Length == 0) return;
        CurrentIndex = (CurrentIndex + 1) % panels.Length;
        ShowPanel(CurrentIndex);
    }

    public void ShowPrev()
    {
        if (panels.Length == 0) return;
        CurrentIndex = (CurrentIndex - 1 + panels.Length) % panels.Length;
        ShowPanel(CurrentIndex);
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
                panels[i].SetActive(i == index);
        }
    }
}