using UnityEngine;

public class EffectHeal : IEffect
{
    [SerializeField] private int _healAmount;
    [SerializeField] private bool _isRateHeal = false;
    private StagePlayer _player;
    public void OnExcute(AttackMagic magic)
    {
        _player = GameManager.Instance.Player;
        if(_isRateHeal)
        {
            int healamount = _player.MaxHP / 100 * _healAmount; 
        }
        else
        {
            _player.Healed(_healAmount);
        }
        CriAudioManager.Instance.PlaySe("SE_Heal");
    }
}
