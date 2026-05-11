using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CardData の Inspector 拡張
/// _canEvolution が true のとき _evolutionID をプルダウンで選択できる
/// </summary>
[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    // ─── SerializedProperty キャッシュ ───────────────────
    private SerializedProperty _cardID;
    private SerializedProperty _rarity;
    private SerializedProperty _type;
    private SerializedProperty _sprite;
    private SerializedProperty _name;
    private SerializedProperty _description;
    private SerializedProperty _cost;
    private SerializedProperty _maxTimes;
    private SerializedProperty _isGhost;
    private SerializedProperty _isDestruction;
    private SerializedProperty _canEvolution;
    private SerializedProperty _evolutionID;
    private SerializedProperty _moveEffect;
    private SerializedProperty _effect;

    // ─── DB 参照 & プルダウン用キャッシュ ────────────────
    private CardDataBase _database;
    private string[] _cardOptions = System.Array.Empty<string>();
    private int[] _cardIdValues;   // 実際の cardId 値
    private int _popupIndex = 0; // プルダウンの選択インデックス

    private void OnEnable()
    {
        _cardID = serializedObject.FindProperty("_cardID");
        _rarity = serializedObject.FindProperty("_rarity");
        _type = serializedObject.FindProperty("_type");
        _sprite = serializedObject.FindProperty("_sprite");
        _name = serializedObject.FindProperty("_name");
        _description = serializedObject.FindProperty("_description");
        _cost = serializedObject.FindProperty("_cost");
        _maxTimes = serializedObject.FindProperty("_maxTimes");
        _isGhost = serializedObject.FindProperty("_isGhost");
        _isDestruction = serializedObject.FindProperty("_isDestruction");
        _canEvolution = serializedObject.FindProperty("_canEvolution");
        _evolutionID = serializedObject.FindProperty("_evolutionID");
        _moveEffect = serializedObject.FindProperty("_moveEffect");
        _effect = serializedObject.FindProperty("_effect");

        // EditorPrefs に保存した DB パスを復元
        string savedPath = EditorPrefs.GetString("CardDataEditor_DatabasePath", "");
        if (!string.IsNullOrEmpty(savedPath))
        {
            _database = AssetDatabase.LoadAssetAtPath<CardDataBase>(savedPath);
            RebuildOptions();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ─── DB セレクター ───────────────────────────────
        EditorGUILayout.Space(4);
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _database = (CardDataBase)EditorGUILayout.ObjectField(
                "Card Database (進化先選択用)", _database, typeof(CardDataBase), false);

            if (check.changed)
            {
                RebuildOptions();
                // 次回も保持
                EditorPrefs.SetString("CardDataEditor_DatabasePath",
                    _database != null ? AssetDatabase.GetAssetPath(_database) : "");
            }
        }
        EditorGUILayout.Space(4);

        // ─── デフォルトフィールドを手動描画 ────────────────
        EditorGUILayout.LabelField("カード詳細", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_cardID);
        EditorGUILayout.PropertyField(_rarity);
        EditorGUILayout.PropertyField(_type);
        EditorGUILayout.PropertyField(_sprite);
        EditorGUILayout.PropertyField(_name);
        EditorGUILayout.PropertyField(_description);
        EditorGUILayout.PropertyField(_cost);
        EditorGUILayout.PropertyField(_maxTimes);
        EditorGUILayout.PropertyField(_isGhost);
        EditorGUILayout.PropertyField(_isDestruction);
        EditorGUILayout.PropertyField(_canEvolution);

        // ─── 進化先 プルダウン（_canEvolution == true のみ表示） ──
        if (_canEvolution.boolValue)
        {
            EditorGUILayout.Space(2);
            using var box = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);

            if (_database == null || _cardOptions.Length == 0)
            {
                // DB 未設定時はフォールバックで IntField
                EditorGUILayout.HelpBox(
                    "Card Database をセットすると進化先をプルダウンで選べます",
                    MessageType.Info);
                EditorGUILayout.PropertyField(_evolutionID);
            }
            else
            {
                // 現在の _evolutionID 値に対応するインデックスを探す
                int currentId = _evolutionID.intValue;
                _popupIndex = FindIndex(currentId);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _popupIndex = EditorGUILayout.Popup(
                        new GUIContent("進化先 (evolutionID)", "Card Database から選択"),
                        _popupIndex, _cardOptions);

                    if (check.changed)
                        _evolutionID.intValue = _cardIdValues[_popupIndex];
                }

                // 現在の ID を補足表示
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.IntField("  ↳ evolutionID", _evolutionID.intValue);
                EditorGUI.EndDisabledGroup();
            }
        }

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("効果", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_moveEffect);
        EditorGUILayout.PropertyField(_effect);

        serializedObject.ApplyModifiedProperties();
    }

    // ─── ヘルパー ────────────────────────────────────────

    /// <summary>DB からプルダウン選択肢を再構築</summary>
    private void RebuildOptions()
    {
        if (_database == null)
        {
            _cardOptions = System.Array.Empty<string>();
            _cardIdValues = System.Array.Empty<int>();
            return;
        }

        var options = new List<string>();
        var ids = new List<int>();

        foreach (var card in _database.Cards)
        {
            if (card == null) continue;
            // 自分自身は進化先として除外
            if (card == target) continue;

            options.Add($"[{card.CardID}]  {card.Name}");
            ids.Add(card.CardID);
        }

        _cardOptions = options.ToArray();
        _cardIdValues = ids.ToArray();

        // 現在値のインデックスを更新
        if (_evolutionID != null)
            _popupIndex = FindIndex(_evolutionID.intValue);
    }

    /// <summary>cardId 値からプルダウンインデックスを返す（見つからなければ 0）</summary>
    private int FindIndex(int id)
    {
        for (int i = 0; i < _cardIdValues.Length; i++)
            if (_cardIdValues[i] == id) return i;
        return 0;
    }
}