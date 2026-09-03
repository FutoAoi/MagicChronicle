using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 攻撃する時に出てくる魔法
/// </summary>
public class AttackMagic : MonoBehaviour
{
    #region 変数宣言
    public int AttackPower = 1;
    public bool IsAttack { get; private set; } = false;

    [SerializeField, Tooltip("タイルの発光色")] private Color _glowingColor;

    [NonSerialized] public MagicVector _currentVector;

    public Vector2Int CurrentSlot => _currentSlot;

    private GameManager _gameManager;
    private AttackManager _attackManager;
    private StagePlayer _player;
    private Enemy _attackedEnemy;
    private Action _onDisable;
    private TileSlot _tileSlot;
    private RectTransform _attackRectTr, _nextRectTr;
    private StageManager _stageManager;
    private Image _slotImg;
    private Color _startColor;
    private Vector2Int _currentSlot, _speedInt;
    private Vector2 _outPos, _goalPos;
    private bool _finish, _firstAttack, _isAttack, _isSelfHarm, _isAccelerate = false,
        _combo = false,_firstParticle = true,_isBreak = false;
    private int _width, _height,_comboStack, _attackIndex = 0;

    #endregion
    #region ライフサイクル
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _attackManager = _gameManager.AttackManager;
        _stageManager = FindAnyObjectByType<StageManager>();
        _width = _gameManager.StageDataBase.GetStageData(_gameManager.StageID).Width;
        _height = _gameManager.StageDataBase.GetStageData(_gameManager.StageID).Height;
        _finish = false;
        _firstAttack = true;
        _isAccelerate = false;
    }
    private void OnDisable()
    {
        
    }
    #endregion
    #region 初期化処理
    public void Initialize(Action onDisable)
    {
        _onDisable = onDisable;
        AttackPower = 1;
        _combo = false;
        _isBreak = false;
    }
    #endregion
    #region 基本挙動
    /// <summary>
    /// 魔法を壊す
    /// </summary>
    public void DestroyMagic(bool isPlayer)
    {
        IsAttack = false;
        _attackManager.AttackFinish(isPlayer);
        _onDisable?.Invoke();
        gameObject.SetActive(false);
    }
    public IEnumerator Attack(Vector2Int startPos, MagicVector startVector, RectTransform startRectTr)
    {
        if (_gameManager.CurrentPhase == BattlePhase.Gameover) yield break;

        IsAttack = true;
        _currentSlot = startPos;//初期ポジ
        bool isPlayer = _attackManager.IsPlayerTurn;
        _player = _gameManager.Player;
        if (_firstParticle)
        {
            AddAttackEffect();
            _firstParticle = false;
        }

        if (isPlayer)
        {
            //プレイヤーのタイプを確認
            if (_gameManager.PlayerType == PlayerType.Combo)
            {
                _combo = true;
                _comboStack = _player.GetBuffCount(BuffType.Combo);
            }

            //筋力バフ適応
            if (_player.HasBuff(BuffType.Strength))
            {
                AttackPower += _player.GetBuffCount(BuffType.Strength);
            }
        }
        else
        {
            if (startPos.x < _stageManager.EnemyList.Count)
            {
                Enemy enemy = _gameManager.StageManager.EnemyList[startPos.x];

                if (enemy != null && !enemy.IsDead)
                {
                    AttackPower = enemy.AttackPower;
                    if(enemy.HasBuff(BuffType.Strength))
                    AttackPower += enemy.GetBuffCount(BuffType.Strength);
                }
            }
        }

        float currentInterval = _attackManager.Interval;

        //移動ループ処理
        while (!_finish)
        {
            CriAudioManager.Instance.PlaySe("SE_MagicMove");
            currentInterval = Mathf.Max(_attackManager.Interval - _attackManager.AttackDeltaTime,0.05f);

            //魔法の移動
            if (_firstAttack)
            {
                _firstAttack = false;
                _currentVector = startVector;
                _attackRectTr = GetComponent<RectTransform>();
                _attackRectTr.position = startRectTr.position;
            }
            if (isPlayer)
            {
                if (_currentSlot.y >= _width)//プレイヤー攻撃成功時
                {
                    _finish = true;
                    _isAttack = true;
                    break;
                }
                else if (_currentSlot.x < 0 || _currentSlot.x > _height - 1)
                {
                    _finish = true;
                    _isAttack = false;
                    _isSelfHarm = false;
                    break;
                }
                else if (_currentSlot.y < 0)
                {
                    _finish = true;
                    _isAttack = false;
                    _isSelfHarm = true;
                    break;
                }
            }
            else
            {
                if (_currentSlot.y < 0)//エネミー攻撃成功時
                {
                    _finish = true;
                    _isAttack = true;
                    break;
                }
                else if (_currentSlot.x < 0 || _currentSlot.x > _height - 1)
                {
                    _finish = true;
                    _isAttack = false;
                    _isSelfHarm = false;
                    break;
                }
                else if (_currentSlot.y >= _width)
                {
                    _finish = true;
                    _isAttack = false;
                    _isSelfHarm = true;
                    break;
                }
            }
            _nextRectTr = _stageManager.SlotList[_currentSlot.x][_currentSlot.y].GetComponent<RectTransform>();
            _attackRectTr.DOMove(_nextRectTr.position, currentInterval)
                .SetEase(Ease.Linear);
            yield return new WaitForSeconds(currentInterval * 0.5f);

            _tileSlot = _stageManager.SlotList[_currentSlot.x][_currentSlot.y].GetComponent<TileSlot>();
            _tileSlot.TileColorChangeAnimation(currentInterval * 0.1f, true);

            yield return new WaitForSeconds(currentInterval * 0.2f);

            //コンボ処理
            if (_combo)
            {
                AttackPower += _comboStack;
            }

            //効果の呼び出し
            if (!_tileSlot.IsOccupied)
            {
                //Slotに何も無いとき
            }
            else
            {
                ActivateMagic(_tileSlot);
            }

            //スロット内部の現在地移動
            _speedInt = GetSpeed(_currentVector);
            _currentSlot += _speedInt;

            //加速処理
            if (_isAccelerate)
            {
                _currentSlot += _speedInt;
                _isAccelerate = false;
            }

            if (isPlayer)
            {
                if (_currentSlot.y >= _width)//プレイヤー攻撃成功時
                {
                    if (!HasCounterBuff(!isPlayer))
                    {
                        _finish = true;
                        _isAttack = true;
                    }
                    else
                    {
                        Enemy target = _stageManager.EnemyList[_currentSlot.x];
                        _attackRectTr.DOMove(target.transform.position, currentInterval)
                            .SetEase(Ease.Linear);
                        yield return new WaitForSeconds(currentInterval);
                        _currentSlot += GetSpeed(_currentVector);
                    }
                }
                else if (_currentSlot.x < 0 || _currentSlot.x > _height - 1)
                {
                    _finish = true;
                    _isAttack = false;
                    _isSelfHarm = false;
                }
                else if (_currentSlot.y < 0)
                {
                    if (!HasCounterBuff(isPlayer))
                    {
                        _finish = true;
                        _isAttack = false;
                        _isSelfHarm = true;
                    }
                    else
                    {
                        _attackRectTr.DOMove(_player.transform.position, currentInterval)
                            .SetEase(Ease.Linear);
                        yield return new WaitForSeconds(currentInterval);
                        _currentSlot += GetSpeed(_currentVector);
                    }
                }
            }
            else
            {
                if (_currentSlot.y < 0)//エネミー攻撃成功時
                {
                    if (!HasCounterBuff(!isPlayer))
                    {
                        _finish = true;
                        _isAttack = true;
                    }
                    else
                    {
                        _attackRectTr.DOMove(_player.transform.position, currentInterval)
                            .SetEase(Ease.Linear);
                        yield return new WaitForSeconds(currentInterval);
                        _currentSlot += GetSpeed(_currentVector);
                    }
                }
                else if (_currentSlot.x < 0 || _currentSlot.x > _height - 1)
                {
                    _finish = true;
                    _isAttack = false;
                    _isSelfHarm = false;
                }
                else if (_currentSlot.y >= _width)
                {
                    if (!HasCounterBuff(isPlayer))
                    {
                        _finish = true;
                        _isAttack = false;
                        _isSelfHarm = true;
                    }
                    else
                    {
                        Enemy target = _stageManager.EnemyList[_currentSlot.x];
                        _attackRectTr.DOMove(target.transform.position, currentInterval)
                            .SetEase(Ease.Linear);
                        yield return new WaitForSeconds(currentInterval);
                        _currentSlot += GetSpeed(_currentVector);
                    }
                }
            }
            yield return new WaitForSeconds(currentInterval * 0.3f);
            _tileSlot.TileColorChangeAnimation(currentInterval * 0.1f, false);
        }

        if (_isAttack)
        {
            if (isPlayer)
            {
                _attackedEnemy = _stageManager.EnemyList[_currentSlot.x];
                _attackRectTr.DOMove(_attackedEnemy.transform.position, currentInterval)
                    .SetEase(Ease.Linear);
                _attackedEnemy.Damaged(AttackPower);
            }
            else
            {
                _attackRectTr.DOMove(_player.transform.position, currentInterval)
                    .SetEase(Ease.Linear);
                _player.Damaged(AttackPower);
            } 
        }
        else
        {
            if (_currentSlot.x < 0) _outPos = new Vector2(0, 1);
            if (_currentSlot.x > _height - 1) _outPos = new Vector2(0, -1);
            if (_currentSlot.y < 0) _outPos = new Vector2(-1, 0);
            if (_currentSlot.y >= _width) _outPos = new Vector2(1, 0);
            _goalPos = _attackRectTr.position;
            if (!_isBreak)
            {
                _goalPos += _outPos * 3f;
            }
            else
            {
                //壊れる演出いれるならここかも
            }
                _attackRectTr.DOMove(_goalPos, currentInterval)
                        .SetEase(Ease.Linear);
        }
        if (_isSelfHarm)
        {
            if (isPlayer)
            {
                _player.Damaged(AttackPower);
            }
            else
            {
                _attackedEnemy = _stageManager.EnemyList[_currentSlot.x];
                _attackedEnemy.Damaged(AttackPower);
            }
        }

        yield return new WaitForSeconds(currentInterval);

        //スロットに接地フラグいれる
        foreach (List<GameObject> Hslot in _stageManager.SlotList)
        {
            foreach (GameObject slot in Hslot)
            {
                _tileSlot = slot.GetComponent<TileSlot>();
                if (_tileSlot.IsOccupied)
                    _tileSlot.IsLastTimeCard = true;
                else _tileSlot.IsLastTimeCard = false;
            }
        }

        _finish = false;
        _firstAttack = true;
        _isSelfHarm = false;
        DestroyMagic(isPlayer);
    }
    /// <summary>
    /// 魔法陣の効果発動
    /// </summary>
    /// <param name="slot"></param>
    private void ActivateMagic(TileSlot slot,int decreaseCount = 1,bool isDecreaseEffect = false,bool isChangeDurability = false)
    {
        CriAudioManager.Instance.PlaySe("SE_MagicCircleWork");
        CardData cardData = _gameManager.CardDataBase.GetCardData(slot.ID);
        if(cardData.MoveEffect != null)
        {
            if (!isChangeDurability)
                cardData.MoveEffect.OnExcute(this);
        }
        foreach (IEffect Effects in cardData.Effect)
        {
            if (cardData.IsGhost)
            {

            }
            else
            {
                if (isDecreaseEffect) continue;
                Effects.OnExcute(this);
            }
        }

        slot.DecreaseTimes(decreaseCount);
    }
    /// <summary>
    /// 反射バフを持っているか確認する。
    /// 結果をbool型で返し、trueならば反射処理を行う。
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    private bool HasCounterBuff(bool isPlayer)
    {
        bool hasBuff = false;
        if (isPlayer)
        {
            hasBuff = _player.HasBuff(BuffType.Counter);
            if(hasBuff)
            {
                _currentVector = MagicVector.Right;
                _player.AddBuff(BuffType.Counter, -1, false);
                CriAudioManager.Instance.PlaySe("SE_MagicReflect");
            }
        }
        else
        {
            hasBuff = _stageManager.EnemyList[_currentSlot.x].HasBuff(BuffType.Counter);
            if(hasBuff)
            {
                _currentVector = MagicVector.Left;
                _stageManager.EnemyList[_currentSlot.x].AddBuff(BuffType.Counter, -1, false);
                CriAudioManager.Instance.PlaySe("SE_MagicReflect");
            }
        }
        return hasBuff;
    }

    private Vector2Int GetSpeed(MagicVector vector)
    {
        switch (vector)
        {
            case MagicVector.UP: return new Vector2Int(-1, 0);
            case MagicVector.Down: return new Vector2Int(1, 0);
            case MagicVector.Left: return new Vector2Int(0, -1);
            case MagicVector.Right: return new Vector2Int(0, 1);
        }
        return Vector2Int.zero;
    }

    public void AddAttackEffect()
    {
        _gameManager.EffectManager.ApplyEffect(ParticleType.Magic, transform);
        _gameManager.EffectManager.ApplyEffect(ParticleType.FireDust, transform);
    }

    public void BeginAttack()
    {
        IsAttack = true;
    }
    #endregion
    #region エフェクト
    /// <summary>
    /// 方向転換
    /// </summary>
    /// <param name="vector"></param>
    public void ChangeVector(MagicVector vector)
    {
        _currentVector = vector;
    }
    /// <summary>
    /// 弾を分割する
    /// </summary>
    /// <param name="vector">進行方向</param>
    /// <param name="start">生成座標</param>
    /// <param name="rect">生成場所</param>
    public void Split(MagicVector vector,Vector2Int start,RectTransform rect)
    {
        StartCoroutine(Attack(start,vector,rect));
    }
    /// <summary>
    /// 次のマスを飛ばす
    /// </summary>
    public void Acceleration()
    {
        _isAccelerate = true;
    }
    /// <summary>
    /// 周囲のマスの耐久値を変化させる
    /// </summary>
    /// <param name="delta">変化量</param>
    public void ChangeAroundDurability(int delta)
    {
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if (i == 1 && j == 1) continue;

                int indexX = _currentSlot.x - 1 + i;
                int indexY = _currentSlot.y - 1 + j;
                if (indexX < 0 || indexY < 0 || indexX > _height - 1 || indexY >= _width) continue;
                if (_stageManager
                    .SlotList[_currentSlot.x - 1 + i][_currentSlot.y - 1 + j]
                    .TryGetComponent<TileSlot>(out var slot))
                {
                    if (slot.IsOccupied)
                    {
                        ActivateMagic(slot, -delta,true,true);
                    }
                }
            }
        }
    }

    public void AttackMagicBreak()
    {
        _finish = true;
        _isBreak = true;
    }
    #endregion
}
