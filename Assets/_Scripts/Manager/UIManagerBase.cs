using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 基本UIManagerクラス
/// </summary>
public abstract class UIManagerBase : MonoBehaviour
{
    public CardMovement CardMovement { get; set; }

    public bool IsDisplayDescription { get; private set; } = false;

    [Header("-----カード-----")]
    [Tooltip("山札")] public List<int> DeckCard = new List<int>();
    [Tooltip("手札")] public List<GameObject> HandCard = new List<GameObject>();
    [Tooltip("捨て札")] public List<int> DiscardCard = new List<int>();
    [Tooltip("除外札")] public List<int> RemoveCard = new List<int>();

    [Header("-----参照-----")]
    [SerializeField, Tooltip("手札の場所")] public Transform HandArea;
    [SerializeField, Tooltip("カードの基盤")] public GameObject CardPrefab;
    [SerializeField, Tooltip("ドラッグ時の場所")] public RectTransform DragLayer;
    [SerializeField, Tooltip("パーティクル用親オブジェクト")] public RectTransform ParticleParent;
    [SerializeField, Tooltip("タイルの暗色")] public Color SelectColor = Color.gray7;
    [SerializeField] private Canvas _canvas;

    [Header("-----説明パネル-----")]
    [SerializeField, Header("親オブジェクト")] private GameObject _descriptionPanel;
    private DescriptionPanel _description;
    protected GameManager _gameManager;
    private RectTransform _rt;

    public bool _isFinishCutIn = false;

    protected virtual void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.RegisterUIManager(this);
        _rt = _descriptionPanel.GetComponent<RectTransform>();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public abstract void InitUI();
    public abstract void UpdateCostUI();


    public void UpdateDescriptionPanel(bool isCard,RectTransform rect,int id = 0,BuffType buff = BuffType.Combo)
    {
        if (!_descriptionPanel.activeSelf) return;

        if (_description == null)
            _description = _descriptionPanel.GetComponent<DescriptionPanel>();

        _rt.position = rect.position;

        if (isCard)
        {
            CardData cardData = _gameManager.CardDataBase.GetCardData(id);
            _description.UpdateCardWindow(cardData);
            foreach (DescriptionKeyWord key in cardData.KeyWords)
            {
                _description.DisplayKeyWordWindow(key);
            }
        }
        else
        {
            BuffData buffData = _gameManager.BuffDataBase.GetBuffData(buff);
            foreach(DescriptionKeyWord key in buffData.KeyWords)
            {
                _description.DisplayKeyWordWindow(key);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rt);
        PositionPanelWithFlip(rect);
    }

    public void DisplayDescriptionPanel(bool isDisplay)
    {
        if (_description == null)
            _description = _descriptionPanel.GetComponent<DescriptionPanel>();

        _description.DisplayWindow(isDisplay);
        IsDisplayDescription = isDisplay;
    }
    public bool IsMouseOverUI(RectTransform rect)
    {
        Vector2 mouse = Pointer.current?.position.ReadValue() ?? Vector2.zero;

        Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : _canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            mouse,
            cam
        );
    }

    private void PositionPanelWithFlip(RectTransform target)
    {
        _rt.position = target.position;
        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);
        Vector3[] panelCorners = new Vector3[4];
        _rt.GetWorldCorners(panelCorners);
        Vector3 offset = Vector3.zero;

        float panelWidth = panelCorners[2].x - panelCorners[0].x;

        if (panelCorners[2].x > canvasCorners[2].x)
        {
            offset.x -= panelWidth * 1.8f;
        }
        else if (panelCorners[0].x < canvasCorners[0].x)
        {
            offset.x += panelWidth * 1.8f;
        }

        if (panelCorners[2].x + offset.x > canvasCorners[2].x)
        {
            offset.x = canvasCorners[2].x - panelCorners[2].x;
        }

        if (panelCorners[0].x + offset.x < canvasCorners[0].x)
        {
            offset.x = canvasCorners[0].x - panelCorners[0].x;
        }

        if (panelCorners[1].y > canvasCorners[1].y)
        {
            offset.y -= (panelCorners[1].y - canvasCorners[1].y);
        }
        
        if (panelCorners[0].y + offset.y < canvasCorners[0].y)
        {
            offset.y += (canvasCorners[0].y - (panelCorners[0].y + offset.y));
        }

        _rt.position += offset;
    }
}
