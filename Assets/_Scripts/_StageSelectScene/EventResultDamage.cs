using UnityEngine;

public class EventResultDamage : EventResultPanelBase
{
    [SerializeField] private TextDisplayAnimation _text;
    public override void ResultAnimation(EventResult result)
    {
        _text.PlayAnimation($"{result.Amount}É_ÉÅÅ[ÉWéÛÇØÇΩ");
    }
}
