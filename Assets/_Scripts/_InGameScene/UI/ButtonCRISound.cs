using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCRISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        CriAudioManager.Instance.PlaySe("SE_ButtonClick");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CriAudioManager.Instance.PlaySe("SE_ButtonHover"); ;
    }
}
