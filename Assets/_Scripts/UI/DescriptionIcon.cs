using UnityEngine;
using UnityEngine.EventSystems;

public class DescriptionIcon : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [Header("-----éQè∆-----")]
    [SerializeField] private DescriptionKeyWord _keyword;
    [SerializeField] private RectTransform _rt;

    private UIManagerBase _manager;

    private void Start()
    {
        _manager = GameManager.Instance.CurrentUIManager;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_manager == null)
        {
            _manager = GameManager.Instance.CurrentUIManager;
        }
        _manager.DisplayDescriptionPanel(true, this);
        _manager.UpdateDescriptionPanel(DescriptionTargetType.Other, _rt, keyword: _keyword);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _manager.DisplayDescriptionPanel(false, this);
    }
}
