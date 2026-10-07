using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Datas/Event")]
public class EventData : ScriptableObject
{
    [SerializeField, Tooltip("イベントID")] private int _eventID;
    [SerializeField, Tooltip("名前")] private string _name;
    [SerializeField, Tooltip("イベントテキスト")] private string _description;
    [SerializeField, Tooltip("背景")] private Sprite _backGround;
    [SerializeField, Tooltip("選択肢")] private EventChoice[] _choices;

    public int EventID => _eventID;
    public string Name => GetLocalizedText("NAME", _name);
    public string Description => GetLocalizedText("DESC", _description);
    public Sprite BackGround => GameManager.Instance.PlayerDataBase.GetPlayerData(GameManager.Instance.PlayerType).EventImage;
    public EventChoice[] Choices => _choices;

    private const string LOCALIZE_TABLE = "MagicChronicle";

    /// <summary>
    /// 現在の言語のテキストをローカライズテーブルから取得する
    /// テーブルにKeyが無い・エディタ非再生時はアセットに入っている文字列を返す
    /// </summary>
    private string GetLocalizedText(string suffix, string fallback)
    {
        if (!Application.isPlaying) return fallback;

        var table = LocalizationSettings.StringDatabase.GetTable(LOCALIZE_TABLE);
        var entry = table != null ? table.GetEntry($"EVENT_{Regex.Replace(name.Replace("Event_", ""), "[^A-Za-z0-9_]", "")}_{suffix}") : null;
        if (entry == null || string.IsNullOrEmpty(entry.Value)) return fallback;
        return entry.GetLocalizedString();
    }
}
