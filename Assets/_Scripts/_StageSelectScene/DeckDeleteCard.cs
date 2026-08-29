using DG.Tweening;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckDeleteCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool IsSelect
    {
        get => _isSelect;
        set
        {
            if (_isSelect == value) return;

            _isSelect = value;

            if (_uiManager == null) _uiManager = GameManager.Instance.CurrentUIManager;
            _uiManager.DisplayDescriptionPanel(_isSelect);
            _uiManager.UpdateDescriptionPanel(true, _rt, _cardID);

            _highLight.SetActive(_isSelect);
        }
    }

    [Header("参照")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _maxTimes;
    [SerializeField, Tooltip("挿絵")] private Image _cardImage;
    [SerializeField] private RectTransform _rt;
    [SerializeField] private GameObject _highLight;
    [SerializeField, Tooltip("矢印色")] private Color _arrowColor = Color.yellowGreen;
    [SerializeField, Tooltip("矢印デフォルト色")] private Color _defaultColor = Color.darkGreen;

    [Header("アニメーション")]
    [SerializeField] private float _selectedScale = 1.1f;
    [SerializeField] private float _duration = 0.2f;

    [Header("矢印")]
    [SerializeField] private Image _up;
    [SerializeField] private Image _right;
    [SerializeField] private Image _left;
    [SerializeField] private Image _down;

    public int DeckIndex { get; private set; }

    private int _cardID;
    private DeckDeletePanel _panel;
    private Vector3 _defaultScale;
    private bool _isSelect;
    private UIManagerBase _uiManager;

    public void SetCard(int cardID, int deckIndex, DeckDeletePanel panel)
    {
        _panel = panel;
        CardData cardData =
            GameManager.Instance.CardDataBase.GetCardData(cardID);

        DeckIndex = deckIndex;
        _defaultScale = transform.localScale;

        _cardID = cardID;
        _name.text = cardData.Name;
        _cost.text = $"{cardData.Cost}";
        _maxTimes.text = $"{cardData.MaxTimes}";
        _cardImage.sprite = cardData.CardSprite;
        _up.color = _defaultColor;
        _down.color = _defaultColor;
        _right.color = _defaultColor;
        _left.color = _defaultColor;
        foreach (MagicVector vector in cardData.DisplayArrowVector)
        {
            GetArrowImage(vector).color = _arrowColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _panel.SelectOrDelete(this);
    }

    public void Select()
    {
        transform.DOKill();
        transform.DOScale(_defaultScale * _selectedScale, _duration)
            .SetEase(Ease.OutBack);

        IsSelect = true;
        _highLight.SetActive(true);
    }

    public void Deselect()
    {
        transform.DOKill();
        transform.DOScale(_defaultScale, _duration)
            .SetEase(Ease.OutQuad);

        IsSelect = false;
        _highLight.SetActive(false);
        _uiManager.DisplayDescriptionPanel(false);
    }

    private Image GetArrowImage(MagicVector vector)
    {
        return vector switch
        {
            MagicVector.UP => _up,
            MagicVector.Right => _right,
            MagicVector.Left => _left,
            MagicVector.Down => _down,
            _ => null
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_uiManager == null) _uiManager = GameManager.Instance.CurrentUIManager;

        _uiManager.DisplayDescriptionPanel(true);
        _uiManager.UpdateDescriptionPanel(true, _rt, _cardID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_uiManager == null) _uiManager = GameManager.Instance.CurrentUIManager;

        _uiManager.DisplayDescriptionPanel(false);
    }
}