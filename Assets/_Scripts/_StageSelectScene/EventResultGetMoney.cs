using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class EventResultGetMoney : EventResultPanelBase
{
    [SerializeField] private TextDisplayAnimation _text;
    public override void ResultAnimation(EventResult result)
    {
        _text.PlayAnimation(LocalizationSettings.StringDatabase.GetLocalizedString("MagicChronicle", "EVENTRESULT_GET_MONEY", new object[] { result.Amount }));
    }
}
