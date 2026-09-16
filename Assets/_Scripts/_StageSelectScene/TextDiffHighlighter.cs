using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 2つの文字列をトークン単位(連続する数字はひとまとまり、それ以外は1文字ずつ)のLCS(最長共通部分列)で比較し、
/// afterの中でbeforeと異なる部分だけをTMPのリッチテキストカラータグで囲む
/// </summary>
public static class TextDiffHighlighter
{
    /// <summary>
    /// beforeとafterを比較し、afterの差分箇所をhighlightColorで色付けした文字列を返す
    /// </summary>
    public static string BuildDiffHighlightedText(string before, string after, Color highlightColor)
    {
        if (string.IsNullOrEmpty(before) || string.IsNullOrEmpty(after))
            return after ?? string.Empty;

        string hex = ColorUtility.ToHtmlStringRGB(highlightColor);
        List<string> beforeTokens = Tokenize(before);
        List<string> afterTokens = Tokenize(after);
        bool[] isCommon = ComputeCommonMask(beforeTokens, afterTokens);

        StringBuilder sb = new StringBuilder();
        bool inHighlight = false;
        for (int i = 0; i < afterTokens.Count; i++)
        {
            if (!isCommon[i] && !inHighlight)
            {
                sb.Append($"<color=#{hex}>");
                inHighlight = true;
            }
            else if (isCommon[i] && inHighlight)
            {
                sb.Append("</color>");
                inHighlight = false;
            }
            sb.Append(afterTokens[i]);
        }
        if (inHighlight) sb.Append("</color>");

        return sb.ToString();
    }

    /// <summary>
    /// 連続する数字はまとめて1トークン、それ以外は1文字ずつ1トークンに分割する
    /// (数字の桁数が変わっても「200」と「300」のように丸ごと1単位として比較するため)
    /// </summary>
    private static List<string> Tokenize(string text)
    {
        List<string> tokens = new();
        int i = 0;
        while (i < text.Length)
        {
            if (char.IsDigit(text[i]))
            {
                int start = i;
                while (i < text.Length && char.IsDigit(text[i])) i++;
                tokens.Add(text.Substring(start, i - start));
            }
            else
            {
                tokens.Add(text[i].ToString());
                i++;
            }
        }
        return tokens;
    }

    /// <summary>
    /// after[i]のトークンが「beforeにも同じ並びで存在した(=変化なし)」かどうかを判定する
    /// </summary>
    private static bool[] ComputeCommonMask(List<string> before, List<string> after)
    {
        int n = before.Count, m = after.Count;
        int[,] dp = new int[n + 1, m + 1];
        for (int i = n - 1; i >= 0; i--)
        {
            for (int j = m - 1; j >= 0; j--)
            {
                dp[i, j] = before[i] == after[j]
                    ? dp[i + 1, j + 1] + 1
                    : Mathf.Max(dp[i + 1, j], dp[i, j + 1]);
            }
        }

        bool[] isCommon = new bool[m];
        int x = 0, y = 0;
        while (x < n && y < m)
        {
            if (before[x] == after[y] && dp[x, y] == dp[x + 1, y + 1] + 1)
            {
                isCommon[y] = true;
                x++; y++;
            }
            else if (dp[x + 1, y] >= dp[x, y + 1])
            {
                x++;
            }
            else
            {
                y++;
            }
        }
        return isCommon;
    }
}
