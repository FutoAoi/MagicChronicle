using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
/// <summary>
/// インゲームのバトル時のUIManager
/// </summary>
public class UIManager_Battle : UIManagerBase, IBattleUI
{
    [Header("-----カード-----")]
    [Tooltip("山札")] public List<int> DeckCard = new List<int>();
    [Tooltip("手札")] public List<GameObject> HandCard = new List<GameObject>();
    [Tooltip("捨て札")] public List<int> DiscardCard = new List<int>();
    [Tooltip("除外札")] public List<int> RemoveCard = new List<int>();

    [Header("-----数値設定-----")]
    [SerializeField, Tooltip("手札の数")] private int _handRange = 5;
    [SerializeField, Tooltip("ドロー間隔")] private float _distance = 0.1f;
    [SerializeField, Tooltip("数字が増える演出時間")] private float _valueDuration = 0.2f;
    [SerializeField, Tooltip("タイルの発行色")] public Color GrowColor = Color.orange;
    [SerializeField, Tooltip("タイルの暗色")] public Color SelectColor = Color.gray7;

    [Header("-----コンポーネント設定-----")]
    [SerializeField, Tooltip("場所")] private RectTransform _playerHandTr;
    [SerializeField, Tooltip("手札の場所")] public Transform HandArea;
    [SerializeField, Tooltip("カードの基盤")] public GameObject CardPrefab;
    [SerializeField, Tooltip("ドラッグ時の場所")] public RectTransform DragLayer;
    [SerializeField, Tooltip("カットインパネル")] private GameObject _enemyAttackPanel;
    [SerializeField, Tooltip("リザルトパネル")] private GameObject _resultPanel;
    [SerializeField, Tooltip("攻撃場所選択パネル")] private GameObject _attackPosPanel;
    [SerializeField, Tooltip("フェード用のパネル")] private Image _fadePanel;
    [SerializeField, Tooltip("デッキ確認用パネル")] private GameObject _deckPanel;
    [SerializeField, Tooltip("ゲームオーバー用パネル")] private GameObject _gameoverPanel;
    [SerializeField, Tooltip("説明パネル")] public GameObject _descriptionPanel;
    [SerializeField, Tooltip("コストのイメージ")] private List<Image> _costImages = new();
    [SerializeField, Tooltip("コストのバックグラウンド")] private List<Image> _costBackGround = new();

    public bool _isFinishCutIn = false;

    private GameManager _gameManager;
    [SerializeField] private StagePlayer _stagePlayer;
    private DeckManager _deckManager;
    private RewardManager _rewardManager;
    private DescriptionPanel _description;
    private GameObject _card;
    private TextMeshProUGUI _text,_descriptionText;
    private Image _panelimg,_descptionImage;
    private RectTransform _panelRectTr;
    private Color _defaultColor;
    private int _currentNumber,_deltaDrawCount = 0;
    public override void InitUI()
    {
        _deckManager = DeckManager.Instance;
        _gameManager = GameManager.Instance;
        _gameManager.CurrentPhase = BattlePhase.BuildStage;
        DeckCard.Clear();
        foreach(int id in _deckManager.DeckMain)
        {
            DeckCard.Add(id);
        }
        HandCard.Clear();
        DiscardCard.Clear();
        RemoveCard.Clear();
        _text = _enemyAttackPanel.GetComponentInChildren<TextMeshProUGUI>();
        _panelimg = _enemyAttackPanel.GetComponent<Image>();
        _panelRectTr = _enemyAttackPanel.GetComponent<RectTransform>();
        _defaultColor = _panelimg.color;
        _enemyAttackPanel.SetActive(false);
        _fadePanel.gameObject.SetActive(false);
        _attackPosPanel.gameObject.SetActive(true);
        _deckPanel.gameObject.SetActive(false);
        _descriptionPanel.gameObject.SetActive(false);
        UpdateMaxCostImage(_stagePlayer.MaxCost);
    }
    public IEnumerator DrawCard()
    {
        for (int i = 0; i < _handRange + _deltaDrawCount; i++)
        {
            CreateCard();
            yield return new WaitForSeconds(_distance);
        }
        SetupCostText();
    }

    public void HandOrganize()
    {
        foreach (var tile in HandCard)
        {
            tile.transform.SetParent(_playerHandTr, false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_playerHandTr);
    }

    public void ClearCard()
    {
        foreach(GameObject hand in HandCard)
        {
            Card card = hand.GetComponent<Card>();
            if (_gameManager.CardDataBase.GetCardData(card.CardID).IsDestruction)
            {
                RegisterRemoveCard(card.CardID);
            }
            else
            {
                ResisterDiscardCard(card.CardID);
            }
            //アニメーションをつくるならここ
            Destroy(hand);
        }
        HandCard.Clear();
    }
    public void ResisterDiscardCard(int id)
    {
        DeckCard.Remove(id);
        DiscardCard.Add(id);
        //アニメーションをつくるならここ
    }
    public void RegisterRemoveCard(int id)
    {
        DeckCard.Remove(id);
        RemoveCard.Add(id);
        //アニメーションをつくるならここ
    }
    public void ResetDeck()
    {
        DeckCard.Clear();
        DeckCard = _deckManager.DeckMain;
        foreach(int id in RemoveCard)
        {
            DeckCard.Remove(id);
        }
        DiscardCard.Clear();
    }
    private void CreateCard()
    {
        _card = Instantiate(CardPrefab, _playerHandTr);
        Card card = _card.GetComponent<Card>();
        card.SetCard(_deckManager.DrawCard(),true);
        DeckCard.Remove(card.CardID);
        HandCard.Add(_card);
    }
    /// <summary>
    /// 敵攻撃時のカットイン
    /// </summary>
    /// <param name="duration"></param>
    public void CutInAnimation(float duration)
    {
        _enemyAttackPanel.SetActive(true);
        _panelRectTr.anchoredPosition = Vector2.right * 1500f;
        _defaultColor = _panelimg.color;
        _panelimg.color = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0f);
        _panelRectTr.transform.localScale = Vector3.one;
        _text.alpha = 0f;
        _text.transform.localScale = Vector3.one * 0.8f;

        Sequence seq = DOTween.Sequence();
        seq.Append(_panelimg.DOFade(1f, duration * 0.12f)).SetEase(Ease.OutQuad);
        seq.Join(_panelRectTr.DOAnchorPos(Vector2.zero, duration * 0.38f).SetEase(Ease.OutExpo));
        seq.Join(_text.DOFade(1f, duration * 0.25f).SetEase(Ease.OutQuad));
        seq.Join(_text.transform.DOScale(1.25f, duration * 0.25f).SetEase(Ease.OutBack));
        seq.AppendInterval(duration * 0.05f);
        seq.Append(_text.transform.DOScale(1f, duration * 0.1f).SetEase(Ease.OutQuad));
        seq.Append(_panelRectTr.DOAnchorPos(Vector2.left * 2500f, duration * 0.2f).SetEase(Ease.InQuad));
        seq.Join(_panelimg.DOFade(0f, duration * 0.15f));
        seq.Join(_text.DOFade(0f, duration * 0.15f));
        seq.OnComplete(() =>
        {
            _isFinishCutIn = true;
            _enemyAttackPanel.SetActive(false);
        });
    }
    /// <summary>
    /// 報酬画面の表示
    /// </summary>
    public void DisplayReward()
    {
        _fadePanel.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f)
            .Append(_fadePanel.DOFade(1f, 0.4f).SetEase(Ease.OutQuad))
            .AppendCallback(() =>
            {
                _resultPanel.SetActive(true);
                _rewardManager = _resultPanel.GetComponent<RewardManager>();
                _rewardManager.Reward();
            })
            .AppendInterval(0.2f)
            .Append(_fadePanel.DOFade(0f, 0.4f))
            .OnComplete(() =>
            {
                _fadePanel.gameObject.SetActive(false);
                StartCoroutine(_rewardManager.RewardAnimation());
            });
    }
    /// <summary>
    /// コスト表記の初期化
    /// </summary>
    public void SetupCostText()
    {
        if (_stagePlayer == null) Debug.Log("playerinai");
        _currentNumber = _stagePlayer.MaxCost;
    }
    /// <summary>
    /// コストテキストの更新
    /// </summary>
    /// <param name="targetValue"></param>
    public void UpdateCostText(int targetValue)
    {
        DOTween.To(() => _currentNumber, 
            x =>
            {
                _currentNumber = x;
                UpdateCostImage(x);
            },
            targetValue,
            _valueDuration
        );
    }

    public void DisplayGameOverPanel()
    {
        _fadePanel.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f)
            .Append(_fadePanel.DOFade(1f, 0.4f).SetEase(Ease.OutQuad))
            .AppendCallback(() =>
            {
                _gameoverPanel.SetActive(true);
            })
            .AppendInterval(0.2f)
            .Append(_fadePanel.DOFade(0f, 0.4f))
            .OnComplete(() =>
            {
                _fadePanel.gameObject.SetActive(false);
            });
    }
    public void UpdateDescriptionPanel(int id, bool isClear)
    {
        if (!_descriptionPanel.activeSelf) return;

        if (_description == null)
            _description = _descriptionPanel.GetComponent<DescriptionPanel>();

        _description.UpdateText(_gameManager.CardDataBase.GetCardData(id),isClear);
    }

    public void DisplayDescriptionPanel(bool isDisplay)
    {
        _descriptionPanel.SetActive(isDisplay);
    }
    /// <summary>
    /// ドロー数を増減させる
    /// </summary>
    /// <param name="delta">変化量</param>
    public void ChangeDrawCount(int delta = 0)
    {
        _deltaDrawCount += delta;
    }

    public void UpdateCostImage(int value)
    {
        foreach(Image cost in _costImages)
        {
            cost.gameObject.SetActive(false);
        }
        for(int i =0; i < value; i++)
        {
            _costImages[i].gameObject.SetActive(true);
        }
    }

    public void UpdateMaxCostImage(int value)
    {
        foreach (Image cost in _costBackGround)
        {
            cost.gameObject.SetActive(false);
        }
        for (int i = 0; i < value; i++)
        {
            _costBackGround[i].gameObject.SetActive(true);
        }
    }

    public override void UpdateCostUI()
    {
        UpdateCostImage(_stagePlayer.CurrentCost);
    }
}
