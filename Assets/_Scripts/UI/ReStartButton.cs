using UnityEngine;

public class ReStartButton : MonoBehaviour
{ 
    public void ReStart()
    {
        GameManager manager = GameManager.Instance;
        manager.PlayerStatus.HealHp(100);
        manager.InitializeBool();
        manager.SceneChange(manager.CurrentScene);
    }
}
