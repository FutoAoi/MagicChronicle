using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCRISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private AttackPointSelectButton _button;
    private void Start()
    {
        _button = GetComponent<AttackPointSelectButton>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        CriAudioManager.Instance.PlaySe("SE_ButtonClick");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button != null && !_button.IsTrandparent)
            CriAudioManager.Instance.PlaySe("SE_ButtonHover"); ;
    }
}
