using UnityEngine;

public class EffectDamagePlayer : IEffect
{
    [SerializeField] private int _damageAmount = 1;
    public void OnExcute(AttackMagic magic)
    {
        GameManager.Instance.Player.Damaged(_damageAmount);
    }
}
