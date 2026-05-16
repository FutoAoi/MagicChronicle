using System;
using UnityEngine;

/// <summary>
/// UŒ‚—Íã¸
/// </summary>
[Serializable]
public class EffectAddPawer : IEffect
{
    [Header("UŒ‚—Íã¸’l")]
    [SerializeField] private int _addPower = 1;
    public void OnExcute(AttackMagic magic)
    {
        if(magic != null)
            magic.AttackPower += _addPower;
    }
}