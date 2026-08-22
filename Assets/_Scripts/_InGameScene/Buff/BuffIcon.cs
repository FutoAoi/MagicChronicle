using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [Header("-----éQè∆-----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _turn;
    [SerializeField] private RectTransform _rt;

    private bool _isDisplayCount = true;
    private GameManager _gameManager;
    private UIManagerBase _uiManager;
    private BuffType _type;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = _gameManager.CurrentUIManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _uiManager.DisplayDescriptionPanel(true);
        _uiManager.UpdateDescriptionPanel(false,_rt,0,_type);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _uiManager.DisplayDescriptionPanel(false);
    }

    public void SetIconData(BuffData data)
    {
        _icon.sprite = data.Icon;
        _isDisplayCount = data.IsDisplayCount;
        _type = data.Type;
    }

    public void UpdateTurn(int turn)
    {
        if (!_isDisplayCount)
        {
            _turn.gameObject.SetActive(false);
            return;
        }
        _turn.text = turn.ToString();
    }
}
