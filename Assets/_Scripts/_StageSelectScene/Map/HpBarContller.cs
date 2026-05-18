using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBarContller : MonoBehaviour
{
    [Header("HPアニメーション設定")]
    [SerializeField, Tooltip("メインバーが減るスピード")] private float _mainSpeed = 0.2f;
    [SerializeField, Tooltip("ゴーストバーが動くまでの時間")] private float _ghostDelay = 0.5f;
    [SerializeField, Tooltip("ゴーストバーが動くスピード")] private float _ghostSpeed = 0.5f;

    [Header("コンポーネント設定")]
    [SerializeField, Tooltip("背景イメージ")] private GameObject _backGround;
    [SerializeField, Tooltip("メインバー")] private Image _mainBar;
    [SerializeField, Tooltip("ゴーストバー")] private Image _ghostBar;
    [SerializeField, Tooltip("HPテキスト")] private TMP_Text _hpText;

    private float _hpRatio;
    private Tween _ghostTween;

    public void HpBarUpdate(int currentHp, int maxHp)
    {
        _hpRatio = (float)currentHp / maxHp;
        _hpText.text = $"{currentHp}/{maxHp}";
        _mainBar.DOFillAmount(_hpRatio, _mainSpeed);
        if (_ghostTween != null && _ghostTween.IsActive())
        {
            _ghostTween.Kill();
        }
        _ghostTween = _ghostBar.DOFillAmount(_hpRatio, _ghostSpeed).SetDelay(_ghostDelay);
    }

    public void ShowUI(int currentHp, int maxHp)
    {
        HpBarUpdate(currentHp, maxHp);
        _backGround.SetActive(true);
    }

    public void HideUI()
    {
        _mainBar.DOFillAmount(0f, _mainSpeed).OnComplete(() => _backGround.SetActive(false));
    }
}
