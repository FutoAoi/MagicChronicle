using UnityEngine;

public class StageSerectHpManager : MonoBehaviour
{
    [SerializeField] HpBarController _hpBarController;
    void Start()
    {
        _hpBarController.ShowUI(GameManager.Instance.PlayerStatus.PlayerCurrentHp, GameManager.Instance.PlayerStatus.PlayerMaxHp);
    }
}
