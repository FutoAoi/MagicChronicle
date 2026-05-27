using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IBuffable
{
    #region 変数宣言
    public bool IsDead;
    public int MaxHP => _maxHP;
    public int CurrentHP => _currentHP;
    [SerializeField, Tooltip("攻撃力")] protected int _attackPower;

    private int _maxHP;
    private int _currentHP;
    private BuffStacks _buffs = new BuffStacks((int)BuffType.End);
    protected GameManager _gameManager;
    protected BuffUIManager _buffUIManager;
    #endregion
    #region ライフサイクル
    protected virtual void Start()
    {
        _gameManager = GameManager.Instance;
        _buffUIManager = GetComponent<BuffUIManager>();
    }
    #endregion
    #region 基本処理

    public void SetStatus(int maxHP, int currentHP)
    {
        _maxHP = maxHP;
        _currentHP = currentHP;
    }
    /// <summary>
    /// HP上限値の上昇
    /// </summary>
    /// <param name="plus"></param>
    public void IncreaseMaxHP(int maxHp)
    {
        _maxHP = maxHp;
    }
    /// <summary>
    /// HPの回復
    /// </summary>
    /// <param name="plus"></param>
    public void RecoveryHP(int plus)
    {
        _currentHP = Mathf.Clamp(_currentHP + plus, 0, _maxHP);
    }
    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="damage">ダメージ数</param>
    public virtual void Damaged(int damage)
    {
        if (IsDead) return;
        _currentHP -= damage;
        if (_currentHP <= 0)
        {
            _currentHP = 0;
            IsDead = true;
            Dead();
        }
    }

    public abstract void Dead();

    #endregion

    #region バフ
    public void AddBuff(BuffType type, int time = 1000)
    {
        if(time == 1000)
        {
            _buffs[type] = 1;
        }
        else
            _buffs[type] = Mathf.Clamp(_buffs[type] + time, 0, 999);

        if (_buffs[type] <= 0)
        {
            _buffUIManager.FalseIcon(type);
            return;
        }
        UpdateBuffImage(_gameManager.BuffDataBase.GetBuffData(type), _buffs[type]);
    }

    public virtual void UpdateBuffImage(BuffData buff,int count)
    {
        _buffUIManager.DisplayBuff(buff.Type, count);
    }
    public void DecreaseAll(int amount = 1)
    {
        _buffs.DecreaseAll(_gameManager.BuffDataBase,amount);
    }
    public bool HasBuff(BuffType type) => _buffs.Has(type);

    public int ReturnBuffCount(BuffType type)
    {
        return _buffs.Stack(type);
    }

    /// <summary>
    /// 攻撃力変化
    /// </summary>
    /// <param name="delta">変化量</param>
    public void AddPower(int delta)
    {
        _attackPower = Mathf.Max(1, _attackPower + delta);
    }

    #endregion
    #region HP関係

    #endregion
}
