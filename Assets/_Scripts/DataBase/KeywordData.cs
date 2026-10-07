using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
[Serializable]
public class KeywordData
{
    public DescriptionKeyWord Type;
    public string KeyName;
    public string Description;
    public Color KeywordColor;

    /// <summary>現在の言語のキーワード名</summary>
    public string LocalizedName => GetLocalizedText("NAME", KeyName);
    /// <summary>現在の言語のキーワード説明</summary>
    public string LocalizedDescription => GetLocalizedText("DESC", Description);

    public string ApplyColor(string description)
    {
        string hex = ColorUtility.ToHtmlStringRGB(KeywordColor);
        return description.Replace(
            KeyName,
            $"<color=#{hex}>{KeyName}</color>"
        );
    }

    private const string LOCALIZE_TABLE = "MagicChronicle";

    /// <summary>
    /// 現在の言語のテキストをローカライズテーブルから取得する
    /// テーブルにKeyが無い・エディタ非再生時はアセットに入っている文字列を返す
    /// </summary>
    private string GetLocalizedText(string suffix, string fallback)
    {
        if (!Application.isPlaying) return fallback;

        var table = LocalizationSettings.StringDatabase.GetTable(LOCALIZE_TABLE);
        var entry = table != null ? table.GetEntry($"KEYWORD_{Type.ToString().ToUpper()}_{suffix}") : null;
        if (entry == null || string.IsNullOrEmpty(entry.Value)) return fallback;
        return entry.GetLocalizedString();
    }
}
