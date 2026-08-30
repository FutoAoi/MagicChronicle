using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("-----éQè∆-----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _turn;
    [SerializeField] private RectTransform _rt;
    [SerializeField] private float _duration = 3f;

    private bool _isDisplayCount = true;
    private GameManager _gameManager;
    private UIManagerBase _uiManager;
    private BuffType _type;
    private Coroutine _coroutine;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        _uiManager.DisplayDescriptionPanel(true);
        _uiManager.UpdateDescriptionPanel(false, _rt, 0, _type);

        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(DelayDisplay());
    }

    private IEnumerator DelayDisplay()
    {
        yield return new WaitForSeconds(_duration);
        _uiManager.DisplayDescriptionPanel(false);
    }
}
