using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance;
    public WalletPanel CurrentWalletPanel;
    public int CurrentMoney => _currentMoney;
    public int CurrentJem => _currentJem;
    [Header("-----数値設定-----")]
    [SerializeField, Header("増加演出時間")] private float _duration = 0.7f;
    [SerializeField, Header("移動待機時間")] private float _waitSec = 0.4f;

    [SerializeField] private int _currentMoney;
    private int _currentJem;
    private Tween _tween;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// プレイヤーの所持金を変化させる。
    /// お金がマイナスになる時は何も起きない。
    /// </summary>
    /// <param name="delta">変化量</param>
    public void ChangePlayerMoney(int delta)
    {
        if (_currentMoney + delta >= 0)
        {
            StartCoroutine(WalletChangeAnimation(WalletType.Money, _currentMoney + delta));
        }
        else
        {
            Debug.Log("お金が" + $"{delta - _currentMoney}" + "＄足りません");
        }
    }

    /// <summary>
    /// お金が足りるかの判定
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool TrySpendMoney(int amount)
    {
        if (_currentMoney < amount) return false;

        StartCoroutine(WalletChangeAnimation(WalletType.Money, _currentMoney - amount));
        return true;
    }
    /// <summary>
    /// プレイヤーのジェム数を変化させる。
    /// ジェム数がマイナスになる時は何も起こらない。
    /// </summary>
    /// <param name="delta"></param>
    public void ChangeJem(int delta)
    {
        if (_currentJem + delta >= 0)
        {
            StartCoroutine(WalletChangeAnimation(WalletType.Jem, _currentJem + delta));
        }
        else
        {
            Debug.Log("ジェムが" + $"{delta - _currentJem}" + "個足りません");
        }
    }

    private IEnumerator WalletChangeAnimation(WalletType type,int targetValue)
    {
        //更新先テキストの取得
        int value = type switch
        {
            WalletType.Money => _currentMoney,
            WalletType.Jem => _currentJem,
            _ => 0
        };

        if (WalletType.Jem == type)
            _currentJem = targetValue;
        else
            _currentMoney = targetValue;

        TextMeshProUGUI _walletText = CurrentWalletPanel.GetWalletText(type);

        //パネル移動演出
        CurrentWalletPanel.PanelMoveAnimation(true);
        yield return new WaitUntil(() => CurrentWalletPanel.IsFinishAnim);

        //テキスト更新演出
        bool isfinish = false;

        _tween?.Kill();
        _tween = DOTween.To(() => value,
            x =>
            {
                value = x;
                _walletText.text = value.ToString();
            },
            targetValue,
            _duration
        )
        .OnComplete(() =>
        {
            isfinish = true;
        });

        yield return new WaitUntil(() => isfinish);
        yield return new WaitForSeconds(_waitSec);

        //パネル移動演出
        CurrentWalletPanel.PanelMoveAnimation(false);
    }
}
