using UnityEngine;

public class EffectAngerDual : IEffect
{
    public void OnExcute(AttackMagic magic)
    {
        GameManager.Instance.BerserkerManager.DoubleAnger();
    }
}
