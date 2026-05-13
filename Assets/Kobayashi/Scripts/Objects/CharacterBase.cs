using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IBuffable
{
    #region 変数宣言
    public bool IsDead;
    public int MaxHP => _maxHP;
    public int CurrentHP => _currentHP;

    private int _maxHP;
    private int _currentHP;
    private BuffStacks _buffs = new BuffStacks((int)BuffType.End);
    protected GameManager _gameManager;
    #endregion
    #region ライフサイクル
    protected virtual void Start()
    {
        _gameManager = GameManager.Instance;
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
    public void AddBuff(BuffType type, int time)
    {
        _buffs[type] = (byte)Mathf.Clamp(_buffs[type] + time,0,255);

        //buffUIMAnagerをBaseで取得するようにしてここでfalse化処理を書く
        if(_buffs[type] <= 0) return;
        UpdateBuffImage(_gameManager.BuffDataBase.GetBuffData(type), _buffs[type]);
    }

    public abstract void UpdateBuffImage(BuffData buff,byte count);
    public void DecreaseAll(byte amount = 1)
    {
        _buffs.DecreaseAll(_gameManager.BuffDataBase,amount);
    }
    public bool HasBuff(BuffType type) => _buffs.Has(type);

    #endregion
    #region HP関係

    #endregion
}
