using UnityEngine;

public class EffectHeal : IEffect
{
    [SerializeField] private int _healAmount;
    private StagePlayer _player;
    public void OnExcute(AttackMagic magic)
    {
        _player = GameManager.Instance.Player;
        _player.Healed(_healAmount);
    }
}
