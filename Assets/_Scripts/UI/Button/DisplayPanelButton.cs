using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DisplayPanelButton : MonoBehaviour
{
    [SerializeField, Tooltip("表示/非表示")] private bool _isShow = true;
    [SerializeField, Tooltip("表示したいパネル")] private RectTransform _panel;
    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Push);
    }
    private void Push()
    {
        _panel.gameObject.SetActive(_isShow);
    }
}
