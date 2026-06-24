using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _tileBoardPrefab;
    [SerializeField] private Sprite[] _tileSprites;
    [NonSerialized] public int ID;
    [SerializeField] private SlotMagicCircleShadow _slotMagicCircleShadow;
    [SerializeField] private RectTransform _windowRt;
    public bool IsLastTimeCard = false;//前のフェーズで置かれたものかどうか
    /// <summary>
    /// すでに置かれているかのフラグ。
    /// set時にタイルの色変更を行う
    /// </summary>
    public bool IsOccupied
    {
        get => _isOccupied;
        set
        {
            if (IsOccupied == value) return;

            _isOccupied = value;

            if (_isOccupied)
            {
                _img.DOColor(_uiManager.SelectColor, 0.1f);
            }
            else
            {
                _slotMagicCircleShadow.DisplayShadow();
                _img.DOColor(Color.white, 0.1f);
            }
        }
    }
    private GameManager _gameManager;
    private GameObject _newCard;
    private CardMovement _tileMovement;
    private UIManager_Battle _uiManager;
    private Image _img,_gauge;
    private RectTransform _rt;
    private Tween _tween;
    private int _index,_currentnumber,_max;
    private bool _isDestroy = false,_isColorChange = false,_isOccupied = false;

    private void Start()
    {
        _img = GetComponent<Image>();
        _rt = GetComponent<RectTransform>();
        _index = UnityEngine.Random.Range(0, _tileSprites.Length - 1);
        _img.sprite = _tileSprites[_index];
        IsLastTimeCard = false;
        _gameManager = GameManager.Instance;
        if(_gameManager.CurrentUIManager.TryGetComponent<UIManager_Battle>(out var manager))
        {
            _uiManager = manager;
        }
    }
    /// <summary>
    /// カードを置く
    /// </summary>
    /// <param name="cardSprite"></param>
    public void PlaceCard(int id)
    {
        if(IsOccupied)return;
        ID = id;
        _newCard = Instantiate(_tileBoardPrefab,transform);
        _tileMovement = _newCard.GetComponent<CardMovement>();
        _tileMovement.ID = id;
        _tileMovement.SetSlotMagicImage(_gameManager.CardDataBase.GetCardData(id));
        _tileMovement.SetAsBoardCard();
        _gauge = _tileMovement.Gauge;
        _currentnumber = _gameManager.CardDataBase.GetCardData(id).MaxTimes;
        _max = _currentnumber;
        IsOccupied = true;
    }
    /// <summary>
    /// スロットの中を無くす
    /// </summary>
    public void ClearSlot()
    {
        if(_newCard != null)
        {
            //壊れる演出
            _tileMovement.MagicDestroyAnimation();
            _newCard = null;
        }
        IsOccupied = false;
    }
    /// <summary>
    /// スロットの使用回数減少
    /// </summary>
    /// <param name="times">使用回数</param>
    public void DecreaseTimes(int times)
    {
        if (!IsOccupied || _isDestroy) return;

        float value = _currentnumber;
        _currentnumber -= times;

        //ゲージ最大値更新処理(回復時)
        if(times < 0)
        {
            if(_gameManager.CardDataBase.GetCardData(ID).MaxTimes < _currentnumber)
            _max = _currentnumber;
        }

        _newCard.GetComponentInChildren<TextMeshProUGUI>().text = _currentnumber.ToString();

        //耐久ゲージ演出
        _tween?.Kill();
        _tween = DOTween.To(() => value,
            x =>
            {
                value = x;
                _gauge.fillAmount = Mathf.Lerp(0f, 1f, value / _max);
            },
            _currentnumber,
            0.2f
        );

        if (_currentnumber <= 0)
        {
            _isDestroy = true;
            CardData data = _gameManager.CardDataBase.GetCardData(ID);
            if (data.IsGhost)
            {
                foreach(IEffect effect in data.Effect)
                {
                    effect.OnExcute(null);
                    CriAudioManager.Instance.PlaySe("SE_MagicCircleWork");
                }
            }
            else
            {
                CriAudioManager.Instance.PlaySe("SE_MagicCircleBreak");
            }

            if (data.IsPlayerMagic)
            {
                _gameManager.EffectManager.ApplyEffect(ParticleType.DestroyFire, _uiManager.ParticleParent, _rt);
                if (data.IsDestruction)
                {
                    (_gameManager.CurrentUIManager as IBattleUI)?.RegisterRemoveCard(ID);
                }
                else
                {
                    (_gameManager.CurrentUIManager as IBattleUI)?.ResisterDiscardCard(ID);
                }
            }
            else
            {
                _gameManager.EffectManager.ApplyEffect(ParticleType.DestroyFireRed, _uiManager.ParticleParent, _rt);
            }
                _isDestroy = false;
            ClearSlot();
        }
    }
    /// <summary>
    /// タイルを発光させる
    /// </summary>
    /// <param name="duration">演出時間</param>
    /// <param name="toGrow">明るくさせるかどうか</param>
    public void TileColorChangeAnimation(float duration,bool toGrow)
    {
        //演出いるようだったら描く
        return;

        //if (_isColorChange) return;

        //Color startColor = _img.color;
        //Color endColor = toGrow ? _uiManager.GrowColor : Color.white;

        //if(startColor == endColor) return;

        //Sequence seq = DOTween.Sequence();

        //seq.Append(_img.DOColor(endColor, duration).SetEase(Ease.Linear))
        //    .OnStart(() => _isColorChange = true)
        //    .OnComplete(() => _isColorChange = false)
        //    .OnKill(() => _isColorChange = false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsOccupied)
        {
            _uiManager.DisplayDescriptionPanel(true);
            _uiManager.UpdateDescriptionPanel(true,_windowRt,ID);
        }
        else
        {
            if(_uiManager.CardMovement != null)
            {
                _slotMagicCircleShadow.DisplayShadow(true,_uiManager.CardMovement.ID);
            }
            _img.DOColor(_uiManager.SelectColor, 0.1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsOccupied)
        {
            _uiManager.DisplayDescriptionPanel(false);
        }
        else
        {
            if (_uiManager.CardMovement != null)
            {
                _slotMagicCircleShadow.DisplayShadow(false, _uiManager.CardMovement.ID);
            }
            _img.DOColor(Color.white, 0.1f);
        }
    }
}
