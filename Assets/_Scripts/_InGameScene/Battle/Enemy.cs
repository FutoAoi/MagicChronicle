using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : CharacterBase
{
    public bool IsAttackTurn => _isAttackTurn;
    public bool IsSpecialAttack => _isSpecialAttack;
    public int EnemyID => _enemyID;
    public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;


    [Header("エネミー詳細")]
    [SerializeField, Tooltip("エネミーの画像")] private Image _enemyImage;
    [SerializeField] private Image _attackImage;
    [SerializeField, Tooltip("攻撃ターンの表示")] private TextMeshProUGUI _attackTurnTMP;
    [SerializeField] private Image _specialImage;
    [SerializeField, Tooltip("特殊攻撃ターンの表示")] private TextMeshProUGUI _specialTMP;
    [SerializeField, Tooltip("お金演出プレハブ")] private GameObject _moneyParticle;

    [SerializeField, Tooltip("エネミーの攻撃までのターン数")] private int _enemyAT;
    private int _enemyID;
    private EnemyData _enemy;
    private bool _isAttackTurn = false;
    private bool _isSpecialAttack = false;
    private int _currentSAT;
    private SkeletonAnimation _skeletonAnimation;
    private GameObject _spineEnemy;

    /// <summary>
    /// エネミーにステータスをセット
    /// </summary>
    /// <param name="enemyID"></param>
    public void SetEnemyStatus(int enemyID)
    {
        _enemyID = enemyID;
        _enemy = GameManager.Instance.EnemyDataBase.GetEnemyData(enemyID);
        SetStatus(_enemy.EnemyHP, _enemy.EnemyHP);
        _attackPower = _enemy.EnemyAP;
        _enemyAT = _enemy.EnemyAT;
        if (_enemy.IsSpecialAttack)
        {
            _currentSAT = _enemy.EnemySAT;
            _specialTMP.text = _currentSAT.ToString();
        }
        else
        {
            _specialTMP.text = null;
        }
        _enemyImage.sprite = _enemy.Sprite;
        _attackTurnTMP.text = _enemyAT.ToString();
        if (enemyID == 0)
        {
            _enemyImage.color = new Color(1f,1f,1f,0f);
            _attackImage.gameObject.SetActive(false);
            _specialImage.gameObject.SetActive(false);
            _attackTurnTMP.text = null;
            _specialTMP.text = null;
            IsDead = true;
        }
        else
        {
            _spineEnemy = Instantiate(_enemy.SpineEnemy, this.transform);
            _skeletonAnimation = _spineEnemy.GetComponent<SkeletonAnimation>();
            HpBarContller.ShowUI(CurrentHP, MaxHP);
        }
    }
    /// <summary>
    /// エネミーに攻撃
    /// </summary>
    /// <param name="damage"></param>
    public override void Damaged(int damage)
    {
        if (IsDead) return;
        base.Damaged(damage);
        _skeletonAnimation.AnimationState.SetAnimation(0, "damage_motion", false);
        _skeletonAnimation.AnimationState.AddAnimation(0, "idle_motion", true, 0);
        CriAudioManager.Instance.PlaySe("SE_MagicHitEnemy");
    }

    /// <summary>
    /// 攻撃ターンを縮める
    /// </summary>
    /// <param name="reductionTurn"></param>
    public void ContractionAttackTurn(int reductionTurn)
    {
        if (IsDead)return;
        _enemyAT -= reductionTurn;
        _attackTurnTMP.text = _enemyAT.ToString();
        if (_enemy.IsSpecialAttack)
        {
            _currentSAT -= reductionTurn;
            _specialTMP.text = _currentSAT.ToString();
            
            if(_currentSAT <= 0)
            {
                _isSpecialAttack = true;
                _currentSAT = _enemy.EnemySAT;
            }
        }
        if(_enemyAT <= 0)
        {
            _isAttackTurn = true;
            _enemyAT = _enemy.EnemyAT;
        }
    }

    /// <summary>
    /// バフ付与
    /// </summary>
    /// <returns></returns>
    public IEnumerator BuffCast()
    {
        if (_enemy.CanBuff && !IsDead)
        {
            foreach (IBuff buff in _enemy.Buffs)
            {
                buff.Excute(this);
                yield return null;
            }
        }
        _gameManager.AttackManager.FinishEnemySpecialAttack(true);
    }
    /// <summary>
    /// 盤面干渉攻撃
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpecialAttack()
    {
        if (_enemy.CanBoardInterference && !IsDead)
        {
            StageData stageData = _gameManager.StageManager.Stage;

            //空のSlotのリスト作成
            List<TileSlot> emptyTiles = new();
            for (int i = 0; i < stageData.Height; i++)
            {
                for (int j = 0; j < stageData.Width; j++)
                {
                    if (_gameManager.StageManager.SlotList[i][j].
                        TryGetComponent<TileSlot>(out var tile))
                    {
                        if (!tile.IsOccupied)
                            emptyTiles.Add(tile);
                    }
                }
            }

            //空Slotリストの中から抽選
            foreach (int cardID in _enemy.CardEffectID)
            {
                if (emptyTiles.Count == 0)
                {
                    Debug.LogWarning("空きマスが足りませんでした");
                    break;
                }

                int index = Random.Range(0, emptyTiles.Count);
                emptyTiles[index].PlaceCard(cardID);
                emptyTiles[index].IsLastTimeCard = true;
                emptyTiles.RemoveAt(index);

                yield return null;
            }
        }
        
        _gameManager.AttackManager.FinishEnemySpecialAttack(false);
    }

    /// <summary>
    /// 攻撃終了
    /// </summary>
    public void FinishAttack()
    {
        _isAttackTurn = false;
        _isSpecialAttack = false;
        if(IsDead)return ;
        _attackTurnTMP.text = _enemyAT.ToString();
        if(_enemy.IsSpecialAttack)
            _specialTMP.text = _currentSAT.ToString();
    }

    public override void Dead()
    {
        HpBarContller.HideUI();
        _attackImage.gameObject.SetActive(false);
        _specialImage.gameObject.SetActive(false);
        _attackTurnTMP.text = null;
        _specialTMP.text = null;
        _spineEnemy.SetActive(false);

        WalletManager.Instance.ChangePlayerMoney(_enemy.RandomReword());

        if(_gameManager.CurrentUIManager.TryGetComponent<UIManagerBase>(out var manager))
        {
            _gameManager.EffectManager.ApplyEffect(ParticleType.Money,manager.ParticleParent,Rect);
        }
    }
}
