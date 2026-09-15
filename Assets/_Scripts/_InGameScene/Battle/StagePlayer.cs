using System.Collections;
using DG.Tweening;
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
        if (!IsDead)
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, "damage_motion", false);
            _skeletonAnimation.AnimationState.AddAnimation(0, "idle_motion", true, 0);
        }
        if(GameManager.Instance.PlayerType == PlayerType.Berserker)
        {
            GameManager.Instance.BerserkerManager.AddAnger(GetBuffCount(BuffType.Berserker));
        }
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
        StartCoroutine(DeadAnimation());
    }
    private IEnumerator DeadAnimation()
    {
        CriAudioManager.Instance.StopBgm();
        CriAudioManager.Instance.PlaySe("SE_HP0");
        var track = _skeletonAnimation.AnimationState.SetAnimation(0, "deth_motion", false);
        yield return new WaitForSeconds(track.Animation.Duration * 5f / 16f);
        CriAudioManager.Instance.PlaySe("SE_PlayerFallDown1");
        yield return new WaitForSeconds(track.Animation.Duration * 5f / 16f);
        CriAudioManager.Instance.PlaySe("SE_PlayerFallDown2");
        yield return new WaitForSeconds(track.Animation.Duration * 6f / 16f);
        _gameManager.CurrentPhase = BattlePhase.Gameover;
        CriAudioManager.Instance.PlaySe("ME_Lose");
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

    public void DamageFromMagicAttacks(int damage, Vector2Int attackPos, AttackMagic magicPrefab = null, float duration = 0.6f, float height = 2.8f)
    {
        RectTransform startRt = _gameManager.StageManager
            .SlotList[attackPos.x][attackPos.y].GetComponent<RectTransform>();
        RectTransform finishRt = GetComponent<RectTransform>();
        AttackMagic attack = MagicObjectPool.Instance.GetAttackMagic(magicPrefab);
        RectTransform magic = attack.GetComponent<RectTransform>();

        Vector3 startPos = startRt.position;
        Vector3 endPos = finishRt.position;
        int width = _gameManager.StageManager.Stage.Width;
        RectTransform farthestRt = _gameManager.StageManager.SlotList[attackPos.x][0].GetComponent<RectTransform>();
        float referenceDistance = Vector3.Distance(farthestRt.position, endPos);
        float distance = Vector3.Distance(startPos, endPos);
        height = height * Mathf.Clamp01(distance / referenceDistance);
        _gameManager.AttackManager.AttackMagicIndex++;
        magic.position = startPos;
        attack.gameObject.SetActive(true);
        attack.AddAttackEffect();
        attack.BeginAttack();
        float direction = (endPos.y >= startPos.y) ? 1f : -1f;
        float t = 0;
        DOTween.To(() => t, x => t = x, 1f, duration)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() =>
            {
                Vector3 linear = Vector3.Lerp(startPos, endPos, t);
                float arcOffset = 4f * height * t * (1f - t) * direction;
                magic.position = linear + new Vector3(0f, arcOffset, 0f);
            })
            .OnComplete(() =>
            {
                Damaged(damage);
                attack.DestroyMagic(_gameManager.AttackManager.IsPlayerTurn);
            });
    }
}
