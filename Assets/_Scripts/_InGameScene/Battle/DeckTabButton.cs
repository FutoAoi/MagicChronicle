using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class DeckTabButton : MonoBehaviour
{
    [Header("-----参照-----")]
    [SerializeField] private InGameDeckType _type;

    [Header("-----アニメーション設定-----")]
    [SerializeField] private float _duration = 0.2f;

    private DeckPanelManager _deckPanelManager;
    private Tween _tween;
    private Image _backImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _deckPanelManager = GetComponentInParent<DeckPanelManager>();
        if (_backImage == null) _backImage = GetComponent<Image>();
        _backImage = GetComponent<Image>();
        ChangeColor(_type == InGameDeckType.Deck);
    }

    public void ChangeTab()
    {
        _deckPanelManager.ChangeDeckTab(_type);
    }

    public void ChangeColor(bool isBraight)
    {
        _tween?.Kill();
        float finish = isBraight ? 1f : 0f;
        Color finishColor = new Color(finish, finish, finish);
        _tween = _backImage.DOColor(finishColor, _duration);
    }
}
