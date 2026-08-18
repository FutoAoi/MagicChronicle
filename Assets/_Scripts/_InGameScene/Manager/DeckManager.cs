using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("メインデッキ(初期データ・テンプレート)")]
    [SerializeField] private DeckData _deckData;

    // ランタイムで実際に書き換える用のコピー
    private DeckData _runtimeDeckData;

    public List<int> DeckMain => _runtimeDeckData.Cards;

    private GameManager _gameManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _gameManager = GameManager.Instance;

        // アセット本体を書き換えないよう、ランタイム用のコピーを作成
        _runtimeDeckData = Instantiate(_deckData);
    }

    /// <summary>
    /// 報酬などでメインのデッキに入れるために使用予定
    /// </summary>
    /// <param name="id"></param>
    public void AddDeck(int id)
    {
        _runtimeDeckData.Cards.Add(id);
    }

    /// <summary>
    /// デッキ内の指定インデックスのカードを強化版に差し替える
    /// </summary>
    /// <param name="deckIndex">DeckMain内でのインデックス(同じIDが複数あるため)</param>
    public bool EnhanceCard(int deckIndex)
    {
        if (deckIndex < 0 || deckIndex >= DeckMain.Count) return false;

        int currentId = DeckMain[deckIndex];
        CardData currentCard = GameManager.Instance.CardDataBase.GetCardData(currentId);

        if (currentCard == null || !currentCard.CanEvolution) return false;

        _runtimeDeckData.Cards[deckIndex] = currentCard.EvolutionID;
        return true;
    }

    /// <summary>
    /// デッキ内のどのインデックスが強化可能かを取得(選択UI用)
    /// </summary>
    public List<int> GetEnhanceableIndices()
    {
        var result = new List<int>();
        for (int i = 0; i < DeckMain.Count; i++)
        {
            CardData card = GameManager.Instance.CardDataBase.GetCardData(DeckMain[i]);
            if (card != null && card.CanEvolution)
            {
                result.Add(i);
            }
        }
        return result;
    }

    /// <summary>
    /// デッキを初期状態(アセットの内容)に戻す。周回プレイやニューゲーム開始時に使用想定。
    /// </summary>
    public void ResetDeck()
    {
        if (_runtimeDeckData != null)
        {
            Destroy(_runtimeDeckData);
        }
        _runtimeDeckData = Instantiate(_deckData);
    }

    private void OnDestroy()
    {
        if (_runtimeDeckData != null)
        {
            Destroy(_runtimeDeckData);
        }
    }
}