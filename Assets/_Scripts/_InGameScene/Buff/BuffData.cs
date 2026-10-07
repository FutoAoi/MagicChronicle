using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
[CreateAssetMenu(menuName = "Datas/Buff")]
public class BuffData : ScriptableObject
{
    [Header("-----基本情報-----")]
    [SerializeField] private BuffType _type;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [TextArea(3, 10)]
    [SerializeField] private string _description;
    [SerializeField] private bool _isDecreaseTurn;
    [SerializeField] private bool _isDisplayCount = true;
    [SerializeReference, SubclassSelector] private IBuff[] _effect;
    [SerializeField] private List<DescriptionKeyWord> _keywords = new();

    public BuffType Type => _type;
    public Sprite Icon => _icon;
    public string Name => GetLocalizedText("NAME", _name);
    public string Description => GetLocalizedText("DESC", _description);
    public bool IsDecreaseTurn => _isDecreaseTurn;
    public bool IsDisplayCount => _isDisplayCount;
    public IBuff[] Effect => _effect;
    public List<DescriptionKeyWord> KeyWords => _keywords;

    private const string LOCALIZE_TABLE = "MagicChronicle";

    /// <summary>
    /// 現在の言語のテキストをローカライズテーブルから取得する
    /// テーブルにKeyが無い・エディタ非再生時はアセットに入っている文字列を返す
    /// </summary>
    private string GetLocalizedText(string suffix, string fallback)
    {
        if (!Application.isPlaying) return fallback;

        var table = LocalizationSettings.StringDatabase.GetTable(LOCALIZE_TABLE);
        var entry = table != null ? table.GetEntry($"BUFF_{_type.ToString().ToUpper()}_{suffix}") : null;
        if (entry == null || string.IsNullOrEmpty(entry.Value)) return fallback;
        return entry.GetLocalizedString();
    }
}
