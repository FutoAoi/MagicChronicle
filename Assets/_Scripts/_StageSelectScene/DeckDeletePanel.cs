using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckDeletePanel : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private DeckDeleteCard _cardPrefab;
    [SerializeField] private GameObject _layPanel;

    private readonly List<DeckDeleteCard> _cardViews = new();

    private DeckDeleteCard _selectedCard;

    private Func<int, bool> _tryDeleteCard;

    /// <summary>
    /// デッキ削除パネルを開く
    /// </summary>
    /// <param name="tryDeleteCard">
    /// 引数はデッキ内インデックス。
    /// 削除成功時はtrue、失敗時はfalseを返す。
    /// </param>
    public void Open(Func<int, bool> tryDeleteCard)
    {
        _tryDeleteCard = tryDeleteCard;
        gameObject.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        _selectedCard = null;
        _tryDeleteCard = null;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 1回目は選択、同じカードの2回目は削除を実行
    /// </summary>
    public void SelectOrDelete(DeckDeleteCard clickedCard)
    {
        if (_selectedCard != clickedCard)
        {
            _selectedCard?.Deselect();

            _selectedCard = clickedCard;
            _selectedCard.Select();
            return;
        }

        if (_tryDeleteCard == null)
            return;

        bool isDeleted = _tryDeleteCard.Invoke(clickedCard.DeckIndex);

        if (!isDeleted)
            return;

        // 1枚削除後、一覧を作り直してインデックスを更新
        _selectedCard = null;
        Close();
    }

    private void Refresh()
    {
        foreach (DeckDeleteCard cardView in _cardViews)
        {
            Destroy(cardView.gameObject);
        }

        _cardViews.Clear();

        List<int> deck = DeckManager.Instance.DeckMain;

        for (int i = 0; i < deck.Count; i++)
        {
            DeckDeleteCard cardView =
                Instantiate(_cardPrefab, _content);

            cardView.SetCard(deck[i], i, this);
            _cardViews.Add(cardView);
        }
    }
}