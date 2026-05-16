using UnityEngine;

/// <summary>
/// Šî–{UIManagerƒNƒ‰ƒX
/// </summary>
public abstract class UIManagerBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        GameManager.Instance.RegisterUIManager(this);
    }
    /// <summary>
    /// ‰Šú‰»
    /// </summary>
    public abstract void InitUI();
    public abstract void UpdateCostUI();
}
