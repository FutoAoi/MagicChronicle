using UnityEngine;

public class EffectUseAnger : IEffect
{
    [SerializeField] private int _useAngerAmount;
    
    public void OnExcute(AttackMagic magic)
    {
        if(GameManager.Instance.BerserkerManager.TryConsumeAnger(_useAngerAmount))
        {
            Effect(magic);
        }
    }

    protected virtual void Effect(AttackMagic magic) { }
}
