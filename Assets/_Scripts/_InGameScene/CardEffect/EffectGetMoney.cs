using System;
using UnityEngine;
/// <summary>
/// ‚¨‹àŠl“¾
/// </summary>
[Serializable]
public class EffectGetMoney : IEffect
{
    [Header("Šl“¾—Ê")]
    [SerializeField] private int _amount = 100;
    public void OnExcute(AttackMagic magic)
    {
        WalletManager.Instance.ChangePlayerMoney(_amount);
    }
}
