using UnityEngine;

public class StageButton : MonoBehaviour
{
    [Header("参照")]
    [SerializeField, Tooltip("表示するパネル")] private GameObject _charactorSelectPanel;
    [SerializeField, Tooltip("キャラクター選択")] private GameObject[] _charactors;
    [SerializeField, Tooltip("チュートリアル選択ボタン")] private GameObject _tutorialButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}
