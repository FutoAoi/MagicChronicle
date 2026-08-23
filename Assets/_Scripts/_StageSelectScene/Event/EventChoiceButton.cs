using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventChoiceButton : MonoBehaviour
{
    [SerializeField, Tooltip("選択肢のボタン")] private Button _button;
    [SerializeField, Tooltip("選択肢のテキスト")] private TMP_Text _choiceText;
    [SerializeField] private Image _img;
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private float _textDuration = 0.3f;

    private EventChoice _eventChoice;
    private EventPanelController _controller;

    public void Setup(EventChoice choice, EventPanelController controller)
    {
        _eventChoice = choice;
        _controller = controller;
        int length = 0;
        string text = choice.ChoiceText;
        _choiceText.text = "";
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
        _img.color = new Color(1f, 1f, 1f, 0f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.8f);
        seq.Append(_img.DOFade(1f, _duration));
        seq.AppendCallback(() =>
        {
            DOTween.To(() => length,
                    x =>
                    {
                        length = x;
                        _choiceText.text = text.Substring(0, length);

                    },
                    text.Length,
                    _textDuration);
        });
    }

    private void OnClick()
    {
        _controller.OnChoiceSelected(_eventChoice);
    }
}
