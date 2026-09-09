using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TextDisplayAnimation : MonoBehaviour
{
    [Header("-----参照-----")]
    [SerializeField] private TextMeshProUGUI _text;

    [Header("-----設定-----")]
    [SerializeField] private float _duration = 1f;
    [SerializeField] private bool _isPlayOnEnable = true;

    [Header("-----アクション登録-----")]
    [SerializeField] private UnityEvent _onFinished;
    [SerializeField] private UnityEvent _nextAction;

    private Tweener _tweener;
    private bool _isPlaying = false,_isClick = false;
    private void OnEnable()
    {
        _isClick = false;
        if(_isPlayOnEnable)
        PlayAnimation(_text.text);
    }

    private void Update()
    {
        if (!_isPlaying && !_isClick) return;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            if (_isPlaying)
            {
                _tweener.Complete();
                return;
            }
            if (_isClick)
            {
                _isClick = false;
                _onFinished?.Invoke();
            }
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
        _isClick = false;
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
                _isClick = true;
                _text.text = text;
                _nextAction?.Invoke();
            });
    }
}
