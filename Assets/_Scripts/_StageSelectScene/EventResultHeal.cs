using UnityEngine;

public class EventResultHeal : EventResultPanelBase
{
    [SerializeField] private TextDisplayAnimation _text;
    public override void ResultAnimation(EventResult result)
    {
        _text.PlayAnimation($"HP‚ª{result.Amount}‰ñ•œ‚µ‚½");
    }
}
