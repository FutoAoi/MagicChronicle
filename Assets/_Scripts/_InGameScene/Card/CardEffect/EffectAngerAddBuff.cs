using UnityEngine;

public class EffectAngerAddBuff : EffectUseAnger
{
    [SerializeField] private BuffType _addBuffType;
    [SerializeField] private int _buffAmount = 1;
    protected override void Effect(AttackMagic magic)
    {
        GameManager.Instance.Player.AddBuff(_addBuffType, _buffAmount);
    }   
}
