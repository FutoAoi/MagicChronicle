using TMPro;
using UnityEngine;

public class WalletPanel : MonoBehaviour
{
    [Header("-----参照-----")]
    [SerializeField, Header("お金テキスト")] private TextMeshProUGUI _moneyText;
    [SerializeField, Header("ジェムテキスト")] private TextMeshProUGUI _jemText;

    private WalletManager _manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager = WalletManager.Instance;
        _manager.CurrentWalletPanel = this;
        _moneyText.text = _manager.CurrentMoney.ToString();
        _jemText.text = _manager.CurrentJem.ToString();
    }
    /// <summary>
    /// テキストを取得
    /// </summary>
    /// <param name="type">お金の種類</param>
    /// <returns></returns>
    public TextMeshProUGUI GetWalletText(WalletType type)
    {
        return type switch
        {
            WalletType.Money => _moneyText,
            WalletType.Jem => _jemText,
            _ => null
        };
    }
}
