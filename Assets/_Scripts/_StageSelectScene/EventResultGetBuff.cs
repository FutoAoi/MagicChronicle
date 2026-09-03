using UnityEngine;
using UnityEngine.UI;

public class EventResultGetBuff : EventResultPanelBase
{
    [SerializeField] private Image _img;
    [SerializeField] private TextDisplayAnimation _text;
    public override void ResultAnimation(EventResult result)
    {
        BuffData data = GameManager.Instance.BuffDataBase.GetBuffData((BuffType)result.ID);
        _img.sprite = data.Icon;
        _text.PlayAnimation($"{data.Name}‚Ì—Í‚ðŽè‚É“ü‚ê‚½");
    }
}
