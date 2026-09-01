using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;

public class StagePlayer : CharacterBase
{
    public int MaxCost => _maxCost = _status.PlayerMaxCost + GetBuffCount(BuffType.CostPlus) - GetBuffCount(BuffType.CostMinus);
    public int CurrentCost => _currentCost;
    public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;

    [SerializeField] private int _maxCost = 8;
    private int _currentCost;
    private PlayerStatus _status;
    private SkeletonAnimation _skeletonAnimation;

    protected override void Start()
    {
        base.Start();

        _gameManager.Player = this;
    }



    public override void Damaged(int damage)
    {
        base.Damaged(damage);
        CriAudioManager.Instance.PlaySe("SE_MagicHitPlayer");
        _skeletonAnimation.AnimationState.SetAnimation(0, "damage_motion", false);
        _skeletonAnimation.AnimationState.AddAnimation(0, "idle_motion", true, 0);
    }

    /// <summary>
    /// コストの初期化
    /// </summary>
    public void SetCost()
    {
        _currentCost = MaxCost;
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
        _gameManager.CurrentPhase = BattlePhase.Gameover;
    }

    public void StagePlayerInit(PlayerStatus nowPlayerStatus)
    {
        _status = nowPlayerStatus;
        foreach (var buff in _status.DefaultBuffs)
        {
            AddBuff(buff, 1, false);
        }
        SetStatus(_status.PlayerMaxHp, _status.PlayerCurrentHp);
        GameObject spinePlayer = Instantiate(_gameManager.PlayerDataBase.GetPlayerData(_gameManager.PlayerType).PlayerPrefab, this.transform);
        _skeletonAnimation = spinePlayer.GetComponent<SkeletonAnimation>();
        HpBarContller.ShowUI(CurrentHP, MaxHP);
        _maxCost = _status.PlayerMaxCost;
    }
}
