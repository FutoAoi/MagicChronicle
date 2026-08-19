using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextDisplayAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = 1f;

    private void OnEnable()
    {
        string text = _text.text;
        _text.text = "";
        int length = 0;
        DOTween.To(() => length,
            x =>
            {
                length = x;
                _text.text = text.Substring(0, length);
            },
            text.Length,
            _duration);
    }
}
