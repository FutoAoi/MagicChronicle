
using UnityEngine;

public class EffectAddAnger : IEffect
{
    [SerializeField] private int _addAmount = 0;
    public void OnExcute(AttackMagic magic)
    {
        GameManager.Instance.BerserkerManager.AddAnger(_addAmount);
    }
}
