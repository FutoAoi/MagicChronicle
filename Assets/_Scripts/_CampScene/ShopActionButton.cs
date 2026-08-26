using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopActionButton : MonoBehaviour, IPointerClickHandler, IShopSelectable
{
    public enum ActionType
    {
        Rest,
        DeleteCard
    }

    [SerializeField] private ActionType _actionType;
    [SerializeField] private float _selectedScale = 1.1f;
    [SerializeField] private float _duration = 0.2f;

    private ShopManager _shopManager;
    private Vector3 _defaultScale;
    private bool _isSelected;

    public void Initialize(ShopManager shopManager)
    {
        _shopManager = shopManager;
        _defaultScale = transform.localScale;
        gameObject.SetActive(true);
        _isSelected = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_shopManager == null)
            return;

        // 1回目: 選択 / 同じものの2回目: 確定
        bool isConfirmed = _shopManager.SelectOrConfirm(this);

        if (!isConfirmed)
        {
            _isSelected = true;

            transform.DOKill();
            transform.DOScale(_defaultScale * _selectedScale, _duration)
                .SetEase(Ease.OutBack);

            return;
        }

        // 2回目クリック後の具体的な処理は ShopManager 側へ
        switch (_actionType)
        {
            case ActionType.Rest:
                _shopManager.Rest();
                break;

            case ActionType.DeleteCard:
                _shopManager.OpenDeleteCardPanel();
                break;
        }
    }

    public void Deselect()
    {
        _isSelected = false;

        transform.DOKill();
        transform.DOScale(_defaultScale, _duration)
            .SetEase(Ease.OutQuad);
    }

    private void OnDisable()
    {
        _shopManager?.ClearSelectedItem(this);
    }
}