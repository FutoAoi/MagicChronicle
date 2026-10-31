using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Datas/Enemy")]
public class EnemyData : ScriptableObject
{
    public int EnemyID => _enemyID;
    public string EnemyName => _enemyName;
    public Sprite Sprite => _sprite;
    public GameObject SpineEnemy => _spineEnemy;
    public int EnemyHP => _enemyHP;
    public int EnemyAP => _enemyAP;
    public int EnemyAttackTime => _enemyAttackTime;
    public int EnemyAT => _enemyAT;
    public bool IsBoss => _isBoss;
    public bool IsShadow => _isShadow;
    public BossType BossType => _bossType;
    public GameObject ShadowPrefab => _shadowPrefab;
    public DefaultBuff[] DefaultBuffs => _defaultBuffs;
    public bool IsSpecialAttack => _isSpecialAttack;
    public int EnemySAT => _enemySAT;
    public bool CanBoardInterference => _canBoardInterference;
    public int EffectTime => _effectTime;
    public int[] CardEffectID => _cardEffectID;
    public bool CanBuff => _canBuff;
    public BuffType[] Buffs => _buffs;
    public bool IsSpecialAttackMagic => _isSpecialAttackMagic;
    public AttackMagic SpecialAttackMagic => _specialAttackMagic;
    public bool IsCallFriend => _isCallFriend;
    public int CallFriendCount => _callFriendCount;
    public int[] CallFriendIDs => _callFriendIDs;
    public int[] RewardAmount => _rewardAmount;


    [SerializeField, Tooltip("ID")] private int _enemyID;
    [SerializeField, Tooltip("名前")] private string _enemyName;
    [SerializeField, Tooltip("見た目")] private Sprite _sprite;
    [SerializeField, Tooltip("スパイン")] private GameObject _spineEnemy;
    [SerializeField, Tooltip("体力")] private int _enemyHP;
    [SerializeField, Tooltip("攻撃力")] private int _enemyAP;
    [SerializeField, Tooltip("攻撃回数")] private int _enemyAttackTime;
    [SerializeField, Tooltip("攻撃ターン")] private int _enemyAT;
    [SerializeField, Tooltip("ボス")] private bool _isBoss = false;
    [SerializeField, ShowIf("_isBoss"),Tooltip("ボスのシャドウ")] private bool _isShadow = false;
    [SerializeField, ShowIf("_isBoss"), Tooltip("ボスの種類")] private BossType _bossType;
    [SerializeField, ShowIf("_isBoss"), Tooltip("判定オブジェクト")] private GameObject _shadowPrefab;

    [Header("-----デフォルトバフ-----")]
    [SerializeField] private DefaultBuff[] _defaultBuffs;

    [Header("-----特殊攻撃-----")]
    [SerializeField, Tooltip("特殊攻撃フラグ")] private bool _isSpecialAttack;
    [SerializeField, ShowIf("_isSpecialAttack"), Tooltip("特殊攻撃ターン")] private int _enemySAT = 3;
    
    [Header("-----盤面干渉-----"),ShowIf("_isSpecialAttack")]
    [SerializeField, Tooltip("エネミーの盤面干渉攻撃フラグ")] private bool _canBoardInterference;
    [SerializeField, ShowIf("_canBoardInterference"), Tooltip("盤面干渉の数")] private int _effectTime;
    [SerializeField, ShowIf("_canBoardInterference"), Tooltip("設置魔法陣")] private int[] _cardEffectID;

    [Header("-----バフ-----"), ShowIf("_isSpecialAttack")]
    [SerializeField, Tooltip("エネミーのバフ攻撃フラグ")] private bool _canBuff;
    [SerializeField, ShowIf("_canBuff"), Tooltip("エネミーのバフ攻撃のターン")] private BuffType[] _buffs;

    [Header("-----特別攻撃魔法-----"), ShowIf("_isSpecialAttack")]
    [SerializeField,Tooltip("特殊攻撃魔法フラグ")] private bool _isSpecialAttackMagic;
    [SerializeField, Tooltip("特殊攻撃魔法")] private AttackMagic _specialAttackMagic;

    [Header("-----仲間呼ぶ-----")]
    [SerializeField, Tooltip("仲間を呼ぶフラグ")] private bool _isCallFriend;
    [SerializeField, ShowIf("_isCallFriend"), Tooltip("呼ぶ仲間の数")] private int _callFriendCount = 2;
    [SerializeField, ShowIf("_isCallFriend"), Tooltip("呼ぶ仲間のID")] private int[] _callFriendIDs;

    [Header("-----報酬-----")]
    [SerializeField, Tooltip("報酬量")] private int[] _rewardAmount = new int[2];


    /// <summary>
    /// ランダムな報酬量を決める
    /// </summary>
    /// <returns>報酬量</returns>
    public int RandomReword()
    {
        return _rewardAmount[UnityEngine.Random.Range(0, _rewardAmount.Length)];
    }
}

[Serializable]
public class DefaultBuff
{
    public BuffType BuffType => _type;
    public int Amount => _amount;
    [SerializeField] private BuffType _type;
    [SerializeField] private int _amount;
}
