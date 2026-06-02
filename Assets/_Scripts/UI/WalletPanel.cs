using DG.Tweening;
using TMPro;
using UnityEngine;

public class WalletPanel : MonoBehaviour
{
    public bool IsFinishAnim => _isFinishAnim;

    [Header("-----参照-----")]
    [SerializeField, Header("お金テキスト")] private TextMeshProUGUI _moneyText;
    [SerializeField, Header("ジェムテキスト")] private TextMeshProUGUI _jemText;

    [Header("-----数値設定-----")]
    [SerializeField, Header("演出時間")] private float _duration = 0.2f;
    [SerializeField, Header("固定値y")] private float _posY;
    [SerializeField, Header("始点x")] private float _startPos;
    [SerializeField, Header("終点x")] private float _finishPos;
    [SerializeField, Header("インゲームで使う？")] private bool _isInGame = true;

    private WalletManager _manager;
    private RectTransform _rt;
    private bool _isFinishAnim = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rt = GetComponent<RectTransform>();
        _rt.anchoredPosition = new Vector2(_startPos, _posY);
        _manager = WalletManager.Instance;
        _manager.CurrentWalletPanel = this;
        _moneyText.text = _manager.CurrentMoney.ToString();
        _jemText.text = _manager.CurrentJem.ToString();
    }
    /// <summary>
    /// テキストを取得 & アニメーションフラグ初期化
    /// </summary>
    /// <param name="type">お金の種類</param>
    /// <returns></returns>
    public TextMeshProUGUI GetWalletText(WalletType type)
    {
        _isFinishAnim = false;

        return type switch
        {
            WalletType.Money => _moneyText,
            WalletType.Jem => _jemText,
            _ => null
        };
    }
    /// <summary>
    /// パネル移動演出
    /// </summary>
    /// <param name="isStart">始まりかどうか</param>
    public void PanelMoveAnimation(bool isStart)
    {
        if (!_isInGame)
        {
            _isFinishAnim = true;
            return;
        }
        float target = isStart? _finishPos : _startPos;

        _rt.DOAnchorPosX(target, _duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isFinishAnim = true;
            });
    }
}
