using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance;
    public int CurrentMoney => _currentMoney;
    public int CurrentJem => _currentJem;
    private int _currentMoney;
    private int _currentJem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null)
        {
            Destroy(Instance);
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
            _currentMoney += delta;
        }
        else
        {
            Debug.Log("お金が" + $"{delta - _currentMoney}" + "＄足りません");
        }
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
            _currentJem += delta;
        }
        else
        {
            Debug.Log("ジェムが" + $"{delta - _currentJem}" + "個足りません");
        }
    }
}
