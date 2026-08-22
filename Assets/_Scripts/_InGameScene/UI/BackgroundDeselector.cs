using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundDeselector : MonoBehaviour, IPointerClickHandler
{
    private IBattleUI _battleUI;
    private UIManagerBase _uiManager;

    private void Start()
    {
        _uiManager = GameManager.Instance.CurrentUIManager;
        if (_uiManager.TryGetComponent<IBattleUI>(out var battle))
        {
            _battleUI = battle;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_uiManager.CardMovement != null) return; // ƒhƒ‰ƒbƒO’†‚Ì“¯‰Ÿ‚µ‘Îô
        _battleUI.ChangeSelectHandCard(null);
        _uiManager.DisplayDescriptionPanel(false);
    }
}
