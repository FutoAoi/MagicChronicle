using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextDisplayAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private UnityEvent _onFinished;

    private void OnEnable()
    {
        PlayAnimation(_text.text);
    }
    /// <summary>
    /// テキスト表示アニメーション。
    /// </summary>
    /// <param name="text">表示したい文章</param>
    public void PlayAnimation(string text)
    {
        _text.text = "";
        int length = 0;
        DOTween.To(() => length,
            x =>
            {
                length = x;
                _text.text = text.Substring(0, length);
            },
            text.Length,
            _duration)
            .OnComplete(() => _onFinished?.Invoke());
    }
}
