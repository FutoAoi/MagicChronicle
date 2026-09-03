using UnityEngine;

public abstract class EventResultPanelBase : MonoBehaviour
{
    protected GameManager _gameManager;
    private void OnEnable()
    {
        _gameManager = GameManager.Instance;
    }
    public abstract void ResultAnimation(EventResult result);
}
