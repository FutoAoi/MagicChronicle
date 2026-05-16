using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventChoiceButton : MonoBehaviour
{
    [SerializeField, Tooltip("選択肢のボタン")] private Button _button;
    [SerializeField, Tooltip("選択肢のテキスト")] private TMP_Text _choiceText;

    private EventChoice _eventChoice;
    private EventPanelController _controller;

    public void Setup(EventChoice choice, EventPanelController controller)
    {
        _eventChoice = choice;
        _controller = controller;
        _choiceText.text = choice.ChoiceText;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _controller.OnChoiceSelected(_eventChoice);
    }
}
