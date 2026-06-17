using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
/// <summary>
/// インゲームのバトル時のUIManager
/// </summary>
public class UIManager_Battle : UIManagerBase, IBattleUI
{
    public CardMovement CardMovement { get; set; }

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
    [SerializeField, Tooltip("山札の枚数テキスト")] private TextMeshProUGUI _deckCountText;
    [SerializeField, Tooltip("捨て札の枚数テキスト")] private TextMeshProUGUI _discardConutText;
    [SerializeField, Tooltip("パーティクル用親オブジェクト")] public RectTransform ParticleParent;

    public bool _isFinishCutIn = false;

    private GameManager _gameManager;
    [SerializeField] private StagePlayer _stagePlayer;
    private DeckManager _deckManager;
    private RewardManager _rewardManager;
    private DescriptionPanel _description;
    private GameObject _card;
    private int _currentNumber,_deltaDrawCount = 0;
    private int _randomIndex;
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
        _enemyAttackPanel.SetActive(false);
        _fadePanel.gameObject.SetActive(false);
        UpdateMaxCostImage(_stagePlayer.MaxCost);
        UpdateDeckCount(0, DeckCard.Count, InGameDeckType.Deck);
        UpdateDeckCount(0, DiscardCard.Count, InGameDeckType.Discard);
        Debug.Log("初期化");
    }
    private void Start()
    {
        _gameManager.EffectManager.ApplyEffect(ParticleType.Firefly, ParticleParent);
    }
    public IEnumerator DrawCardAnimation()
    {
        int deckCount = DeckCard.Count;
        int discardCount = DiscardCard.Count;
        for (int i = 0; i < _handRange + _deltaDrawCount; i++)
        {
            CreateCard();
            yield return new WaitForSeconds(_distance);
        }
        SetupCostText();


        UpdateDeckCount(deckCount,DeckCard.Count,InGameDeckType.Deck);
        UpdateDeckCount(discardCount, DiscardCard.Count, InGameDeckType.Discard);
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
        Debug.Log("デッキの補充");
        DeckCard.Clear();
        DeckCard = new List<int>(_deckManager.DeckMain);
        foreach(GameObject card in HandCard)
        {
            DeckCard.Remove(card.GetComponent<Card>().CardID);
        }
        DiscardCard.Clear();
    }
    private void CreateCard()
    {
        _card = Instantiate(CardPrefab, _playerHandTr);
        Card card = _card.GetComponent<Card>();
        card.SetCard(DrawCard(),true);
        HandCard.Add(_card);
        HandCardColorChange();
    }
    /// <summary>
    /// 敵攻撃時のカットイン
    /// </summary>
    /// <param name="duration"></param>
    public void CutInAnimation(float duration)
    {
        _isFinishCutIn = false;
        _enemyAttackPanel.SetActive(true);
        _enemyAttackPanel.GetComponent<CutInPanel>().CutInAnimation(duration);
    }
    /// <summary>
    /// 報酬画面の表示
    /// </summary>
    public void DisplayReward()
    {
        FadeManager.Instance.FadePanel(false, () =>
        {
            _resultPanel.SetActive(true);
            _rewardManager = _resultPanel.GetComponent<RewardManager>();
            _rewardManager.Reward();
            FadeManager.Instance.FadePanel(true, () =>
            {
                StartCoroutine(_rewardManager.RewardAnimation());
            });
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

        //手札の明るさ変える
        HandCardColorChange();
    }

    public void DisplayGameOverPanel()
    {
        FadeManager.Instance.FadePanel(false, () =>
        {
            _gameoverPanel.SetActive(true);
            _gameManager.transform.SetAsLastSibling();
            FadeManager.Instance.FadePanel(true);
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
        Debug.Log(_handRange + _deltaDrawCount);
    }

    public void ResetDrawCount()
    {
        _deltaDrawCount = 0;
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

    /// <summary>
    /// デッキカウントの表示更新
    /// </summary>
    /// <param name="start">始めの枚数</param>
    /// <param name="target">終わりの枚数</param>
    /// <param name="deckType">どこを更新するか</param>
    public void UpdateDeckCount(int start,int target,InGameDeckType deckType)
    {
        TextMeshProUGUI text = deckType switch
        {
            InGameDeckType.Deck => _deckCountText,
            InGameDeckType.Discard => _discardConutText,
            _ => null
        };

        if (text == null) return;

        DOTween.To(() => start,
            x =>
            {
                start = x;
                text.text = start.ToString();
            },
            target,
            _valueDuration
            )
            .SetEase(Ease.OutQuad);
    } 

    private void HandCardColorChange()
    {
        foreach (GameObject obj in HandCard)
        {
            Card card = obj.GetComponent<Card>();
            obj.GetComponent<CardHoverAnimation>()
                .ColorAnimation(_gameManager.Player.CurrentCost
                >= _gameManager.CardDataBase.GetCardData(card.CardID).Cost);
        }
    }

    /// <summary>
    /// デッキのシャッフルメソット
    /// </summary>
    public void ShuffleDeck()
    {
        for (int i = 0; i < DeckCard.Count; i++)
        {
            _randomIndex = Random.Range(i, DeckCard.Count);

            int _temp = DeckCard[i];
            DeckCard[i] = DeckCard[_randomIndex];
            DeckCard[_randomIndex] = _temp;
        }
    }

    /// <summary>
    /// デッキドローメッソト
    /// </summary>
    /// <returns></returns>
    public int DrawCard()
    {
        if (DeckCard.Count == 0)
        {
            ResetDeck();
            ShuffleDeck();
        }

        int topCard = DeckCard[0];
        DeckCard.RemoveAt(0);
        return topCard;
    }

    public void RegisterCardMovement(bool isRegist,CardMovement movement)
    {
        CardMovement = isRegist? movement : null;
    }
}
