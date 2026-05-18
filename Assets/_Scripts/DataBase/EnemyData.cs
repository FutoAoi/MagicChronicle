using UnityEngine;

[CreateAssetMenu(menuName = "Datas/Enemy")]
public class EnemyData : ScriptableObject
{
    public int EnemyID => _enemyID;
    public Sprite Sprite => _sprite;
    public int EnemyHP => _enemyHP;
    public int EnemyAP => _enemyAP;
    public int EnemyAttackTime => _enemyAttackTime;
    public int EnemyAT => _enemyAT;
    public bool IsSpecialAttack => _isSpecialAttack;
    public int EnemySAT => _enemySAT;
    public bool CanBoardInterference => _canBoardInterference;
    public int EffectTime => _effectTime;
    public CardData[] CardEffects => _cardEffects;
    public bool CanBuff => _canBuff;
    public IBuff[] Buffs => _buffs;
    public int[] RewardAmount => _rewardAmount;


    [SerializeField, Tooltip("ID")] private int _enemyID;
    [SerializeField, Tooltip("見た目")] private Sprite _sprite;
    [SerializeField, Tooltip("体力")] private int _enemyHP;
    [SerializeField, Tooltip("攻撃力")] private int _enemyAP;
    [SerializeField, Tooltip("攻撃回数")] private int _enemyAttackTime;
    [SerializeField, Tooltip("攻撃ターン")] private int _enemyAT;

    [Header("-----特殊攻撃-----")]
    [SerializeField, Tooltip("特殊攻撃フラグ")] private bool _isSpecialAttack;
    [SerializeField, ShowIf("_isSpecialAttack"), Tooltip("特殊攻撃ターン")] private int _enemySAT = 3;
    
    [Header("-----盤面干渉-----"),ShowIf("_isSpecialAttack")]
    [SerializeField, Tooltip("エネミーの盤面鑑賞攻撃フラグ")] private bool _canBoardInterference;
    [SerializeField, ShowIf("_canBoardInterference"), Tooltip("盤面干渉の数")] private int _effectTime;
    [SerializeField, ShowIf("_canBoardInterference"), Tooltip("設置魔法陣")] private CardData[] _cardEffects;

    [Header("-----バフ-----"), ShowIf("_isSpecialAttack")]
    [SerializeField, Tooltip("エネミーのバフ攻撃フラグ")] private bool _canBuff;
    [SerializeField, ShowIf("_canBuff"), Tooltip("エネミーのバフ攻撃のターン")] private IBuff[] _buffs;

    [Header("-----報酬-----")]
    [SerializeField, Tooltip("報酬量")] private int[] _rewardAmount = new int[2];


    /// <summary>
    /// ランダムな報酬量を決める
    /// </summary>
    /// <returns>報酬量</returns>
    public int RandomReword()
    {
        return _rewardAmount[Random.Range(0, _rewardAmount.Length)];
    }
}
