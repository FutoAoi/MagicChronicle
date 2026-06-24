using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutInPanel : MonoBehaviour
{
    [SerializeField, Tooltip("移行先のフェーズ")] private BattlePhase _battlePhase;
    [SerializeField] private Image _fade;
    [SerializeField] private float _alpha = 0.3f;
    [SerializeField] private Image _magicCircle;
    [SerializeField] private Image _charactorText;
    [SerializeField] private Image _attackText;
    [SerializeField] private Image _line;
    private GameManager _gamemanager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _gamemanager = GameManager.Instance;
    }
    /// <summary>
    /// カットインアニメーション
    /// </summary>
    public void CutInAnimation(float duration)
    {
        CriAudioManager.Instance.PlaySe("SE_TurnSwitch");
        _charactorText.rectTransform.anchoredPosition = new Vector2(Vector2.left.x * 1500f,_charactorText.rectTransform.anchoredPosition.y) ;
        _attackText.rectTransform.anchoredPosition = new Vector2(Vector2.right.x * 1500f,_attackText.rectTransform.anchoredPosition.y);
        _magicCircle.color = new Color(_magicCircle.color.r, _magicCircle.color.g, _magicCircle.color.b, 0f);
        _charactorText.color = new Color(1f,1f,1f,0f);
        _attackText.color = new Color(1f, 1f, 1f, 0f);
        _charactorText.transform.localScale = Vector3.one * 0.8f;
        _attackText.transform.localScale = Vector3.one * 0.8f;
        _magicCircle.transform.localScale = Vector3.one * 0.2f;
        _line.transform.localScale = Vector3.one * 0.2f;
        _fade.color = new Color(0f, 0f, 0f, 0f);
        _line.color = new Color(1f, 1f, 1f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Append(_magicCircle.DOFade(1f, duration * 0.12f)).SetEase(Ease.OutQuad);
        seq.Join(_fade.DOFade(_alpha, duration * 0.1f)).SetEase(Ease.OutQuad);
        seq.Join(_magicCircle.transform.DOScale(1f, duration * 0.12f)).SetEase(Ease.OutQuad);
        seq.Join(_line.transform.DOScale(1f, duration * 0.12f)).SetEase(Ease.OutQuad);
        seq.Join(_line.DOFade(1f, duration * 0.12f)).SetEase(Ease.OutQuad);
        seq.Join(_charactorText.rectTransform.DOAnchorPosX(0f, duration * 0.38f).SetEase(Ease.OutExpo));
        seq.Join(_attackText.rectTransform.DOAnchorPosX(0f, duration * 0.38f).SetEase(Ease.OutExpo));
        seq.Join(_charactorText.DOFade(1f, duration * 0.25f).SetEase(Ease.OutQuad));
        seq.Join(_attackText.DOFade(1f, duration * 0.25f).SetEase(Ease.OutQuad));
        seq.Join(_charactorText.transform.DOScale(1.25f, duration * 0.25f).SetEase(Ease.OutBack));
        seq.Join(_attackText.transform.DOScale(1.25f, duration * 0.25f).SetEase(Ease.OutBack));
        seq.AppendInterval(duration * 0.05f);
        seq.Append(_charactorText.transform.DOScale(1f, duration * 0.1f).SetEase(Ease.OutQuad));
        seq.Join(_attackText.transform.DOScale(1f, duration * 0.1f).SetEase(Ease.OutQuad));
        seq.Append(_charactorText.rectTransform.DOAnchorPosX(Vector2.right.x * 2500f, duration * 0.2f).SetEase(Ease.InQuad));
        seq.Join(_attackText.rectTransform.DOAnchorPosX(Vector2.left.x * 2500f, duration * 0.2f).SetEase(Ease.InQuad));
        seq.Join(_magicCircle.DOFade(0f, duration * 0.15f));
        seq.Join(_fade.DOFade(0f, duration * 0.15f));
        seq.Join(_charactorText.DOFade(0f, duration * 0.15f));
        seq.Join(_attackText.DOFade(0f, duration * 0.15f));
        seq.Join(_line.DOFade(0f, duration * 0.15f));
        seq.Join(_magicCircle.transform.DOScale(0.2f, duration * 0.15f)).SetEase(Ease.OutQuad);
        seq.Join(_line.transform.DOScale(0.2f, duration * 0.15f)).SetEase(Ease.OutQuad);
        seq.OnComplete(() =>
        {
            _gamemanager.CurrentPhase = _battlePhase;
            if(_gamemanager.CurrentUIManager.TryGetComponent<UIManager_Battle>(out var manager))
            {
                manager._isFinishCutIn = true;
            }
            gameObject.SetActive(false);
        });
    }
}
