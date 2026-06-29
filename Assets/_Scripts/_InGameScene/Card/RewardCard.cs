using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("カード詳細")]
    [SerializeField, Tooltip("カードのレアリティ")] private CardRarity _rarity;
    [SerializeField, Tooltip("カードID")] private int _cardID;
    [SerializeField, Tooltip("カードの見た目")] private Image _image;
    [SerializeField, Tooltip("カードの名前")] private TMP_Text _name;
    [SerializeField, Tooltip("カードのコスト")] private TMP_Text _cost;
    [SerializeField, Tooltip("最大回数")] private TMP_Text _maxTimes;
    [SerializeField, Tooltip("報酬番号")] private int _rewardNumber;
    [SerializeField, Tooltip("回転時間")] private float _duration = 0.8f;
    [SerializeField, Tooltip("カード前面")] private GameObject _cardFrontObj;
    [SerializeField, Tooltip("カードの裏面")] private GameObject _cardBackObj;

    [Header("-----アニメーション-----")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _durationScale = 0.2f;

    public int CardID => _cardID;
    public bool IsFinish { get; private set; } = false;

    private CardData _data;
    private Transform _tf;
    private Vector3 _defaultScale;
    private float _selectedScale = 1.1f;
    private float _animationTime = 0.2f;

    private void Awake()
    {
        _tf = transform;
        _cardFrontObj.SetActive(false);
        _cardBackObj.SetActive(true);
    }


    /// <summary>
    /// カード情報をセットする
    /// </summary>
    /// <param name="ID"></param>
    public void SetCard(int ID)
    {
        _data = GameManager.Instance.CardDataBase.GetCardData(ID);
        _cardID = ID;
        _rarity = _data.Rarity;
        _image.sprite = _data.CardSprite;
        _name.text = _data.Name;
        _cost.text = $"{_data.Cost}";
        _maxTimes.text = $"{_data.MaxTimes}";
        _defaultScale = _tf.localScale;
    }

    /// <summary>
    /// カードをめくるアニメーション
    /// </summary>
    public void TurnCardAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_tf.DORotate(Vector2.zero, _duration).SetEase(Ease.Linear));
        seq.InsertCallback(_duration * 0.5f, () =>
        {
            _cardBackObj.SetActive(false);
            _cardFrontObj.SetActive(true);
        });
        seq.OnComplete(() =>
        {
            IsFinish = true;
        });
        
    }

    public void ChangeEncyclopediaCard()
    {
        _cardFrontObj.SetActive(true);
        _cardBackObj.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tf.DOKill();
        _tf.DOScale(_defaultScale * _hoverScale, _durationScale).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tf.DOKill();
        _tf.DOScale(_defaultScale, _durationScale).SetEase(Ease.OutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DeckManager.Instance.AddDeck(_cardID);
        gameObject.SetActive(false);
        GameManager.Instance.SceneChange(SceneType.StageSerectScene);
    }
}
