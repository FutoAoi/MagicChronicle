using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// デッキエディターウィンドウ
/// メニュー: CardGame > Deck Editor
/// </summary>
public class DeckEditorWindow : EditorWindow
{
    // ─── 参照 ───────────────────────────────────────────
    private CardDataBase _database;
    private DeckData _deck;

    // ─── UI 状態 ─────────────────────────────────────────
    private int _selectedCardIndex = 0;
    private Vector2 _deckScrollPos;
    private string _searchFilter = "";

    // プルダウン用キャッシュ
    private string[] _cardOptions = System.Array.Empty<string>();
    private List<CardData> _filteredCards = new List<CardData>();

    // ─── ウィンドウ起動 ──────────────────────────────────
    [MenuItem("CardGame/Deck Editor")]
    public static void Open()
    {
        var win = GetWindow<DeckEditorWindow>("Deck Editor");
        win.minSize = new Vector2(420, 500);
    }

    // ─── GUI ─────────────────────────────────────────────
    private void OnGUI()
    {
        DrawHeader();
        EditorGUILayout.Space(4);

        if (_database == null || _deck == null)
        {
            DrawSetupPanel();
            return;
        }

        DrawAddCardPanel();
        EditorGUILayout.Space(6);
        DrawDeckList();
        EditorGUILayout.Space(6);
        DrawFooter();
    }

    // ─── ヘッダー ────────────────────────────────────────
    private void DrawHeader()
    {
        using var headerScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🃏  Deck Editor", EditorStyles.boldLabel);

        using var changeCheck = new EditorGUI.ChangeCheckScope();

        _database = (CardDataBase)EditorGUILayout.ObjectField(
            "Card Database", _database, typeof(CardDataBase), false);

        _deck = (DeckData)EditorGUILayout.ObjectField(
            "Deck Data", _deck, typeof(DeckData), false);

        if (changeCheck.changed)
            RebuildCardOptions();
    }

    // ─── セットアップ誘導 ────────────────────────────────
    private void DrawSetupPanel()
    {
        EditorGUILayout.HelpBox(
            "Card Database と Deck Data をセットしてください。\n" +
            "右クリック > Create > CardGame から作成できます。",
            MessageType.Info);
    }

    // ─── カード追加パネル ────────────────────────────────
    private void DrawAddCardPanel()
    {
        using var box = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
        EditorGUILayout.LabelField("カードを追加", EditorStyles.boldLabel);

        using (var fc = new EditorGUI.ChangeCheckScope())
        {
            _searchFilter = EditorGUILayout.TextField("🔍 検索", _searchFilter);
            if (fc.changed)
                RebuildCardOptions();
        }

        if (_filteredCards.Count == 0)
        {
            EditorGUILayout.HelpBox("該当カードがありません", MessageType.Warning);
            return;
        }

        _selectedCardIndex = Mathf.Clamp(_selectedCardIndex, 0, _filteredCards.Count - 1);

        using (new EditorGUILayout.HorizontalScope())
        {
            _selectedCardIndex = EditorGUILayout.Popup(
                "カード選択", _selectedCardIndex, _cardOptions);

            if (GUILayout.Button("追加", GUILayout.Width(60)))
                AddSelectedCard();
        }

        // 選択中カードのプレビュー
        var preview = _filteredCards[_selectedCardIndex];
        if (preview != null)
        {
            using var indent = new EditorGUI.IndentLevelScope(1);
            EditorGUILayout.LabelField(
                $"ID: {preview.CardID}   Cost: {preview.Cost}   {preview.Description}",
                EditorStyles.miniLabel);
        }
    }

    // ─── デッキ一覧 ─────────────────────────────────────
    private void DrawDeckList()
    {
        using var box = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);

        int count = _deck.Cards.Count;
        EditorGUILayout.LabelField(
            $"デッキ: {_deck.DeckName}  ({count} 枚)", EditorStyles.boldLabel);

        if (count == 0)
        {
            EditorGUILayout.HelpBox("カードが追加されていません", MessageType.None);
            return;
        }

        _deckScrollPos = EditorGUILayout.BeginScrollView(
            _deckScrollPos, GUILayout.MaxHeight(300));

        int removeIndex = -1;
        for (int i = 0; i < _deck.Cards.Count; i++)
        {
            int id = _deck.Cards[i];
            // int IDからカード名を逆引き（表示用）
            CardData card = _database != null ? _database.GetCardData(id) : null;

            using var row = new EditorGUILayout.HorizontalScope();

            EditorGUILayout.LabelField($"{i + 1,3}.", GUILayout.Width(32));
            EditorGUILayout.LabelField(
                card != null ? card.Name : "(不明)", GUILayout.MinWidth(100));
            EditorGUILayout.LabelField(
                id.ToString(), EditorStyles.miniLabel, GUILayout.Width(60));

            if (card != null && GUILayout.Button("⏎", GUILayout.Width(26)))
                EditorGUIUtility.PingObject(card);

            if (GUILayout.Button("✕", GUILayout.Width(26)))
                removeIndex = i;
        }

        if (removeIndex >= 0)
        {
            Undo.RecordObject(_deck, "Remove Card from Deck");
            _deck.Cards.RemoveAt(removeIndex);
            EditorUtility.SetDirty(_deck);
        }

        EditorGUILayout.EndScrollView();
    }

    // ─── フッター ────────────────────────────────────────
    private void DrawFooter()
    {
        using var row = new EditorGUILayout.HorizontalScope();

        if (GUILayout.Button("全削除"))
        {
            if (EditorUtility.DisplayDialog(
                "確認", $"デッキ「{_deck.DeckName}」の全カードを削除しますか？", "削除", "キャンセル"))
            {
                Undo.RecordObject(_deck, "Clear Deck");
                _deck.Cards.Clear();
                EditorUtility.SetDirty(_deck);
            }
        }

        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(_deck);
            AssetDatabase.SaveAssets();
            Debug.Log($"[DeckEditor] Saved: {_deck.DeckName} ({_deck.Cards.Count} 枚)");
        }
    }

    // ─── ヘルパー ────────────────────────────────────────

    private void RebuildCardOptions()
    {
        _filteredCards.Clear();
        if (_database == null) return;

        string filter = _searchFilter.ToLower();
        foreach (var card in _database.Cards)
        {
            if (card == null) continue;
            if (!string.IsNullOrEmpty(filter))
            {
                // int IDは ToString() で検索
                bool match = card.Name.ToLower().Contains(filter)
                          || card.CardID.ToString().Contains(filter);
                if (!match) continue;
            }
            _filteredCards.Add(card);
        }

        _cardOptions = new string[_filteredCards.Count];
        for (int i = 0; i < _filteredCards.Count; i++)
        {
            var c = _filteredCards[i];
            _cardOptions[i] = $"[{c.CardID}]  {c.Name}";
        }

        _selectedCardIndex = 0;
    }

    private void AddSelectedCard()
    {
        if (_filteredCards.Count == 0) return;
        var card = _filteredCards[_selectedCardIndex];
        if (card == null) return;

        Undo.RecordObject(_deck, "Add Card to Deck");
        _deck.Cards.Add(card.CardID);   // int で保存
        EditorUtility.SetDirty(_deck);

        Debug.Log($"[DeckEditor] Added: {card.CardID} ({card.Name})  合計 {_deck.Cards.Count} 枚");
    }
}