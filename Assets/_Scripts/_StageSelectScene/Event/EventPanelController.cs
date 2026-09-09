using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelController : MonoBehaviour
{
    [Header("-----イベントパネルの設定-----")]
    [SerializeField, Tooltip("背景画像")] private Image _backgroundImage;
    [SerializeField, Tooltip("イベントの名前")] private TMP_Text __eventNameText;
    [SerializeField, Tooltip("説明のテキスト")] private TMP_Text _descriptionText;
    [SerializeField, Tooltip("結果表示テキスト")] private TMP_Text _resultText;
    [SerializeField, Tooltip("結果表示テキスト")] private TextDisplayAnimation _resultEventText;
    [SerializeField, Tooltip("選択肢の場所")] private Transform _choiceButtonParent;
    [SerializeField, Tooltip("選択ボタンのプレハブ")] private EventChoiceButton _choiceButtonPrehab;
    [SerializeField, Tooltip("結果後にとじるボタン")] private Button _closeButton;
    [SerializeField] private HpBarController _hpBarController;
    [SerializeField] private GameObject[] _falseObjs;

    [Header("-----結果パネル-----")]
    [SerializeField] private EventResultPanelBase _cardResultPanel;
    [SerializeField] private EventResultPanelBase _buffResultPanel;
    [SerializeField] private EventResultPanelBase _goldResultPanel;
    [SerializeField] private EventResultPanelBase _healResultPanel;
    [SerializeField] private EventResultPanelBase _damageResultPanel;

    [Header("-----データ-----")]
    [SerializeField, Tooltip("イベントのデータベース")] private EventDataBase _eventDataBase;

    [Header("-----コンポーネント設定-----")]
    [SerializeField, Tooltip("プレイヤーに位置の更新")] private MapView _mapView;

    private EventData _currentEventData;
    private List<EventResult> _pendingResults = new();
    private List<IEventEffect> _pendingEffects = new();
    private int _expectedPanelFinishCount;
    private int _finishedPanelCount;

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
    private void OnDisable()
    {
        _descriptionText.gameObject.SetActive(false);
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
        _descriptionText.gameObject.SetActive(false);
        foreach (Transform child in _choiceButtonParent)
        {
            Destroy(child.gameObject);
        }
        foreach(var choice in _currentEventData.Choices)
        {
            var btn = Instantiate(_choiceButtonPrehab, _choiceButtonParent);
            btn.Setup(choice,this);
        }
        foreach(GameObject obj in _falseObjs)
        {
            obj.SetActive(false);
        }
    }

    public void OnChoiceSelected(EventChoice choice)
    {
        HideAllResultPanels();
        _closeButton.gameObject.SetActive(false);
        _pendingResults.Clear();
        _pendingEffects.Clear();
        foreach (var effect in choice.EventEffects)
        {
            if (effect == null) continue;
            EventResult result = effect.OnExcute();
            _pendingResults.Add(result);
            _pendingEffects.Add(effect);
        }
        foreach(Transform child in _choiceButtonParent)
        {
            child.gameObject.SetActive(false);
        }

        _resultText.text = choice.ResultText;
        _resultText.gameObject.SetActive(true);
        _hpBarController.HpBarUpdate(GameManager.Instance.PlayerStatus.PlayerCurrentHp, GameManager.Instance.PlayerStatus.PlayerMaxHp);
        _resultText.gameObject.SetActive(true);
        _resultEventText.PlayAnimation(choice.ResultText);
    }

    private void ClosePanel()
    {
        FadeManager.Instance.FadePanel(false, () =>
        {
            HideAllResultPanels();
            gameObject.SetActive(false);
            _mapView.UpdataPlayerPosition();
            FadeManager.Instance.FadePanel(true);
        });
    }

    public void EventTextAnimation()
    {
        _descriptionText.gameObject.SetActive(true);
    }

    private bool ShowResultPanel(EventResult result)
    {
        EventResultPanelBase panel = result.Type switch
        {
            EventResultType.Card => _cardResultPanel,
            EventResultType.Buff => _buffResultPanel,
            EventResultType.Gold => _goldResultPanel,
            EventResultType.Heal => _healResultPanel,
            EventResultType.Damage => _damageResultPanel,
            _ => null
        };
        if (panel == null) return false;

        panel.gameObject.SetActive(true);
        panel.ResultAnimation(result);
        return true;
    }

    private void HideAllResultPanels()
    {
        _cardResultPanel.gameObject.SetActive(false);
        _buffResultPanel.gameObject.SetActive(false);
        _goldResultPanel.gameObject.SetActive(false);
        _healResultPanel.gameObject.SetActive(false);
        _damageResultPanel.gameObject.SetActive(false);
    }

    public void OnResultTextFinished()
    {
        _finishedPanelCount = 0;
        _expectedPanelFinishCount = 0;
        for (int i = 0; i < _pendingResults.Count; i++)
        {
            if (ShowResultPanel(_pendingResults[i]))
            {
                _pendingEffects[i].PlaySE();
                _expectedPanelFinishCount++;
            }
        }

        if (_expectedPanelFinishCount == 0)
        {
            _closeButton.gameObject.SetActive(true);
        }
    }

    public void OnResultPanelTextFinished()
    {
        _finishedPanelCount++;
        if (_finishedPanelCount >= _expectedPanelFinishCount)
        {
            _closeButton.gameObject.SetActive(true);
        }
    }
}
