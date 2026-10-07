using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = ("Datas/Choice"))]
public class EventChoice : ScriptableObject
{
    [SerializeField, Tooltip("選択肢の文")] private string _choiceText;
    [SerializeField, Tooltip("結果文")] private string _resultText;
    [SerializeReference, SubclassSelector] private IEventEffect[] _eventEffects;

    public string ChoiceText => GetLocalizedText("TEXT", _choiceText);
    public string ResultText => GetLocalizedText("RESULT", _resultText);
    public IEventEffect[] EventEffects => _eventEffects;

    private const string LOCALIZE_TABLE = "MagicChronicle";

    /// <summary>
    /// 現在の言語のテキストをローカライズテーブルから取得する
    /// テーブルにKeyが無い・エディタ非再生時はアセットに入っている文字列を返す
    /// </summary>
    private string GetLocalizedText(string suffix, string fallback)
    {
        if (!Application.isPlaying) return fallback;

        var table = LocalizationSettings.StringDatabase.GetTable(LOCALIZE_TABLE);
        var entry = table != null ? table.GetEntry($"EVENTCHOICE_{Regex.Replace(name, "[^A-Za-z0-9_]", "")}_{suffix}") : null;
        if (entry == null || string.IsNullOrEmpty(entry.Value)) return fallback;
        return entry.GetLocalizedString();
    }
}
