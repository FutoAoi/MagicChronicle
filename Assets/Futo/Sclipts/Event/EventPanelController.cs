using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelController : MonoBehaviour
{
    [Header("イベントパネルの設定")]
    [SerializeField, Tooltip("背景画像")] private Image _backgroundImage;
    [SerializeField, Tooltip("イベントの名前")] private TMP_Text __eventNameText;
    [SerializeField, Tooltip("説明のテキスト")] private TMP_Text _descriptionText;
    [SerializeField, Tooltip("結果表示テキスト")] private TMP_Text _resultText;
    [SerializeField, Tooltip("選択肢の場所")] private Transform _choiceButtonParent;
    [SerializeField, Tooltip("選択ボタンのプレハブ")] private EventChoiceButton _choiceButtonPrehab;
    [SerializeField, Tooltip("結果後にとじるボタン")] private Button _closeButton;

    [Header("データ")]
    [SerializeField, Tooltip("イベントのデータベース")] private EventDataBase _eventDataBase;

    [Header("コンポーネント設定")]
    [SerializeField, Tooltip("プレイヤーに位置の更新")] private MapView _mapView;

    private EventData _currentEventData;

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake()
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(ClosePanel);
        _closeButton.gameObject.SetActive(false);
        _resultText.gameObject.SetActive(false);
    }

    /// <summary>
    /// イベントIDによるイベントを表示
    /// </summary>
    /// <param name="eventID"></param>
    public void SetupEvent(int eventID)
    {
        gameObject.SetActive(true);
        _currentEventData = _eventDataBase.GetEventData(eventID);
        if (_currentEventData == null ) return;

        _backgroundImage.sprite = _currentEventData.BackGround;
        __eventNameText.text = _currentEventData.Name;
        _descriptionText.text = _currentEventData.Description;
        _closeButton.gameObject.SetActive(false);
        _resultText.gameObject.SetActive(false);
        foreach(Transform child in _choiceButtonParent)
        {
            Destroy(child.gameObject);
        }
        foreach(var choice in _currentEventData.Choices)
        {
            var btn = Instantiate(_choiceButtonPrehab, _choiceButtonParent);
            btn.Setup(choice,this);
        }
    }

    public void OnChoiceSelected(EventChoice choice)
    {
        foreach (var effect in choice.EventEffects)
        {
            effect?.OnExcute();
        }
        foreach(Transform child in _choiceButtonParent)
        {
            child.gameObject.SetActive(false);
        }

        _resultText.text = choice.ResultText;
        _resultText.gameObject.SetActive(true);
        _closeButton.gameObject.SetActive(true);
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
        _mapView.UpdataPlayerPosition();
    }
}
