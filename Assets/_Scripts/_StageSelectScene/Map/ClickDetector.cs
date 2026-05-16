using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SceneType _sceneType;
    public void OnPointerClick(PointerEventData eventData)
    {

        GameManager.Instance.SceneChange(_sceneType);
    }
}
