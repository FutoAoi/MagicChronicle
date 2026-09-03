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
    [SerializeField] private bool _isPlayOnEnable = true;

    private Tweener _tweener;
    private bool _isPlaying = false,_isClick = false;
    private string _string;
    private void OnEnable()
    {
        _isClick = false;
        if(_isPlayOnEnable)
        PlayAnimation(_text.text);
    }

    private void Update()
    {
        if (!_isPlaying) return;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            if (!_isClick)
            {
                _isClick = true;
                _text.text = _string;
                return;
            }
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
        _string = text;
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
                _isClick = true;
                _text.text = text;
                _onFinished?.Invoke();
            });
    }
}
