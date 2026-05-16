using System;
using UnityEngine;

/// <summary>
/// •ûŒü“]Š·
/// </summary>
[Serializable]
public class EffectChangeToAnyAllow : IEffect
{
    [SerializeField] private MagicVector _vector;

    public void OnExcute(AttackMagic magic)
    {
        if (magic != null)
            magic.ChangeVector(_vector);
    }
}