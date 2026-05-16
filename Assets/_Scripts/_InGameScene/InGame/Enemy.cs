using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : CharacterBase
{
    public bool IsAttackTurn => _isAttackTurn;
    public bool IsSpecialAttack => _isSpecialAttack;

    [Header("エネミー詳細")]
    [SerializeField, Tooltip("エネミーの画像")] private Image _enemyImage;
    [SerializeField, Tooltip("攻撃ターンの表示")] private TextMeshProUGUI _attackTurnTMP;
    [SerializeField, Tooltip("特殊攻撃ターンの表示")] private TextMeshProUGUI _specialTMP;

    [SerializeField, Tooltip("エネミーの攻撃までのターン数")] private int _enemyAT;
    [SerializeField] private HpBarContller _hpBarContller;
    private EnemyData _enemy;
    private bool _isAttackTurn = false;
    private bool _isSpecialAttack = false;
    private RectTransform _rect;
    private int _currentSAT;

    /// <summary>
    /// エネミーにステータスをセット
    /// </summary>
    /// <param name="enemyID"></param>
    public void SetEnemyStatus(int enemyID)
    {
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
        _rect = GetComponent<RectTransform>();
        _enemyImage.sprite = _enemy.Sprite;
        _attackTurnTMP.text = _enemyAT.ToString();
        if (enemyID == 0)
        {
            _enemyImage.color = new Color(1f,1f,1f,0f);
            _attackTurnTMP.text = null;
            _specialTMP.text = null;
            IsDead = true;
        }
        else
        {
           _hpBarContller.ShowUI(CurrentHP, MaxHP);
        }
        Debug.Log("Enemy" + $"{CurrentHP}");
    }
    /// <summary>
    /// エネミーに攻撃
    /// </summary>
    /// <param name="damage"></param>
    public override void Damaged(int damage)
    {
        if (IsDead) return;
        base.Damaged(damage);
        DamagePopUpObjectPool.Instance.Get(_rect.anchoredPosition + new Vector2(Random.Range(-50f, 50f), 0f), damage);
        _hpBarContller.HpBarUpdate(CurrentHP, MaxHP);
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
            foreach (CardData card in _enemy.CardEffects)
            {
                if (emptyTiles.Count == 0)
                {
                    Debug.LogWarning("空きマスが足りませんでした");
                    break;
                }

                int index = Random.Range(0, emptyTiles.Count);
                emptyTiles[index].PlaceCard(card.CardID);
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

    /// <summary>
    /// 死亡コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeadIEnumerator()
    {
        yield return null;
        _attackTurnTMP.text = null;
        _specialTMP.text = null;
        _enemyImage.DOFade(0f,0.1f);

        _gameManager.Player.GetMoney(_enemy.RandomReword());
    }

    public override void Dead()
    {
        _hpBarContller.HideUI();
        _attackTurnTMP.text = null;
        _specialTMP.text = null;
        _enemyImage.DOFade(0f, 1f);

        _gameManager.Player.GetMoney(_enemy.RandomReword());
    }
}
