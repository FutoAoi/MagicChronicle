using System;
using UnityEngine;

[Serializable]
public class EffectDebug : IEffect
{
    public void OnExcute(AttackMagic magic)
    {
        Debug.Log("”­“®");
    }
}