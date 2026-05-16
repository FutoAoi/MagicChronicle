using System;
using UnityEngine;
/// <summary>
/// ŽüˆÍ‚Ìƒ}ƒX‚Ì‘Ï‹v’l‚ð•Ï‰»‚³‚¹‚é
/// </summary>
[Serializable]
public class EffectChangeDurability : IEffect
{
    [Header("‘Ï‹v’l‚Ì•Ï‰»—Ê")]
    [SerializeField] private int _delta;
    public void OnExcute(AttackMagic magic)
    {
        if(magic == null) return;

        magic.ChangeAroundDurability(_delta);
    }
}
