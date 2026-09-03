using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TextDisplayAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private UnityEvent _onFinished;

    private Tweener _tweener;
    private bool _isPlaying = false;
    private void OnEnable()
    {
        PlayAnimation(_text.text);
    }

    private void Update()
    {
        if (!_isPlaying) return;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            _tweener.Complete();
        }
    }

    /// <summary>
    /// テキスト表示アニメーション。
    /// </summary>
    /// <param name="text">表示したい文章</param>
    public void PlayAnimation(string text)
    {
        if (_isPlaying) _tweener.Kill();
        _text.text = "";
        int length = 0;
        _isPlaying = true;
        _tweener = DOTween.To(() => length,
            x =>
            {
                length = x;
                _text.text = text.Substring(0, length);
            },
            text.Length,
            _duration)
            .OnComplete(() =>
            {
                _isPlaying = false;
                _text.text = text;
                _onFinished?.Invoke();
            });
    }
}
