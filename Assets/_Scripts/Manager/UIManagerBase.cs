using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本UIManagerクラス
/// </summary>
public abstract class UIManagerBase : MonoBehaviour
{
    [Header("-----カード-----")]
    [Tooltip("山札")] public List<int> DeckCard = new List<int>();
    [Tooltip("手札")] public List<GameObject> HandCard = new List<GameObject>();
    [Tooltip("捨て札")] public List<int> DiscardCard = new List<int>();
    [Tooltip("除外札")] public List<int> RemoveCard = new List<int>();

    protected virtual void Awake()
    {
        GameManager.Instance.RegisterUIManager(this);
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public abstract void InitUI();
    public abstract void UpdateCostUI();
}
