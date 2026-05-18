using UnityEngine;

public class StagePlayer : CharacterBase
{
    public int MaxCost => _maxCost;
    public int CurrentCost => _currentCost;
    public int Money => _money;

    [SerializeField] private int _maxCost = 8;
    [SerializeField] private HpBarContller _hpBarContller;
    private int _money = 0;
    private int _currentCost;
    private RectTransform _rect;

    protected override void Start()
    {
        base.Start();

        _gameManager.Player = this;
        SetStatus(100, 100);
        _rect = GetComponent<RectTransform>();
        _hpBarContller.ShowUI(CurrentHP, MaxHP);
        Debug.Log("Player" + $"{CurrentHP}");
        AddBuff(_gameManager.GivePlayerBuffData().Type,0);
    }

    public override void Damaged(int damage)
    {
        base.Damaged(damage);
        DamagePopUpObjectPool.Instance.Get(_rect.anchoredPosition + new Vector2(Random.Range(-50f, 50f), 0f), damage);
        _hpBarContller.HpBarUpdate(CurrentHP, MaxHP);
    }

    /// <summary>
    /// コスト上限値の上昇
    /// </summary>
    /// <param name="plus"></param>
    public void IncreaseMaxCost(int plus)
    {
        _maxCost += plus;
    }

    /// <summary>
    /// コストの初期化
    /// </summary>
    public void SetCost()
    {
        _currentCost = _maxCost;
        _gameManager.CurrentUIManager.UpdateCostUI();
    }

    /// <summary>
    /// コスト消費できるかどうか
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    public bool ConsumeCost(int cost)
    {
        return _currentCost >= cost;
    }

    /// <summary>
    /// コストの更新
    /// </summary>
    /// <param name="cost">変化量</param>
    /// <param name="isConsume">減らす？</param>
    public void ChangeCost(int cost, bool isConsume)
    {
        if (isConsume)
        {
            _currentCost -= cost;
        }
        else
        {
            _currentCost += cost;
        }
    }

    /// <summary>
    /// お金を支払う
    /// </summary>
    /// <param name="pay"></param>
    public void PayMoney(int pay)
    {

        if (_money - pay <= 0)
        {
            Debug.Log("お金が足りないよ！！！");
            return;
        }
        _money -= pay;
    }

    /// <summary>
    /// お金を手に入れる
    /// </summary>
    /// <param name="plus"></param>
    public void GetMoney(int plus)
    {
        _money += plus;
    }

    public override void Dead()
    {
        _gameManager.CurrentUIManager.GetComponent<UIManager_Battle>().DisplayGameOverPanel();
    }
}
