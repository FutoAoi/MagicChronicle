using TMPro;
using UnityEngine;

public class EventResultGetMoney : EventResultPanelBase
{
    [SerializeField] private TextDisplayAnimation _text;
    public override void ResultAnimation(EventResult result)
    {
        _text.PlayAnimation($"{result.Amount}G‚ðŽè‚É“ü‚ê‚½");
    }
}
