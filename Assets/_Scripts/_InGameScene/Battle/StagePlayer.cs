using UnityEngine;

public class StagePlayer : CharacterBase
{
    public int MaxCost => _maxCost;
    public int CurrentCost => _currentCost;

    [SerializeField] private int _maxCost = 8;
    [SerializeField] private HpBarContller _hpBarContller;
    private int _currentCost;
    private RectTransform _rect;
    private PlayerStatus _status;

    protected override void Start()
    {
        base.Start();

        _gameManager.Player = this;
        _rect = GetComponent<RectTransform>();
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
    public void ChangeMaxCost()
    {
        _maxCost = _status.PlayerMaxCost + GetBuffCount(BuffType.CostPlus) - GetBuffCount(BuffType.CostMinus);
        _gameManager.CurrentUIManager.UpdateCostUI();
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

    public override void Dead()
    {
        _gameManager.CurrentUIManager.GetComponent<UIManager_Battle>().DisplayGameOverPanel();
    }

    public void StagePlayerInit(PlayerStatus nowPlayerStatus)
    {
        _status = nowPlayerStatus;
        SetStatus(_status.PlayerMaxHp, _status.PlayerCurrentHp);
        _hpBarContller.ShowUI(CurrentHP, MaxHP);
        _maxCost = _status.PlayerMaxCost;
        foreach (var buff in _status.DefaultBuffs)
        {
            AddBuff(buff);
        }
    }
}
