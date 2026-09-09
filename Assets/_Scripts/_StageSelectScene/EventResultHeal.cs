using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EventResultHeal : EventResultPanelBase
{
    [SerializeField] private GameObject _hp;
    [SerializeField] private HpBarController _gauge;
    [SerializeField] private TextDisplayAnimation _text;
    private PlayerStatus _status;
    private void OnEnable()
    {
        _hp.SetActive(false);
    }
    private void OnDisable()
    {
        _hp.SetActive(true);
    }
    public override void ResultAnimation(EventResult result)
    {
        _status = GameManager.Instance.PlayerStatus;
        _text.PlayAnimation($"HP‚ª{result.Amount}‰ñ•œ‚µ‚½");
        _gauge.ShowUI(_status.PlayerCurrentHp - result.Amount, _status.PlayerMaxHp);
        _gauge.HpBarUpdate(_status.PlayerCurrentHp,_status.PlayerMaxHp);
    }
}
