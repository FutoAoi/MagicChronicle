using System;
using UnityEngine;
[Serializable]
public class KeywordData
{
    public DescriptionKeyWord Type;
    public string KeyName;
    public string Description;
    public Color KeywordColor;

    public string ApplyColor(string description)
    {
        string hex = ColorUtility.ToHtmlStringRGB(KeywordColor);
        return description.Replace(
            KeyName,
            $"<color=#{hex}>{KeyName}</color>"
        );
    }
}
