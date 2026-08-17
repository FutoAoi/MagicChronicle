using System;
using UnityEngine;
[Serializable]
public class EffectRock : IEffect
{
    public void OnExcute(AttackMagic magic)
    {
        magic.AttackMagicBreak();
    }
}
