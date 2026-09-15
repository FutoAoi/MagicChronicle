using UnityEngine;

public class EffectDamagePlayer : IEffect
{
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private AttackMagic _attackMagic;
    public void OnExcute(AttackMagic magic)
    {
        GameManager.Instance.Player.DamageFromMagicAttacks(_damageAmount, magic.CurrentSlot, _attackMagic);
    }
}
