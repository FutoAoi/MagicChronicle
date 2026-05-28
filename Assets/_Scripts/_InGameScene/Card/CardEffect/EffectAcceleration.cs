using System;
using UnityEngine;
/// <summary>
/// ŽŸ‚Ìˆêƒ}ƒX”ò‚Î‚·
/// </summary>
[Serializable]
public class EffectAcceleration : IEffect
{
    public void OnExcute(AttackMagic magic)
    {
        if(magic != null)
            magic.Acceleration();
    }
}
