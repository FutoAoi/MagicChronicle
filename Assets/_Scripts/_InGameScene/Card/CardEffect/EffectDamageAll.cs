using System.Collections.Generic;
using UnityEngine;

public class EffectDamageAll : IEffect
{
    [SerializeField] private int _damageAmount = 1;
    private List<Enemy> _attackTargets = new();
    public void OnExcute(AttackMagic magic)
    {
        GameManager.Instance.Player.Damaged(_damageAmount);
        CriAudioManager cri = CriAudioManager.Instance;
        _attackTargets = GameManager.Instance.StageManager.EnemyList.FindAll(enemy => enemy.IsDead != true);
        if (_attackTargets.Count == 0) return;
        for (int i = 0; i < _attackTargets.Count; i++)
        {
            _attackTargets[i].Damaged(_damageAmount);
        }
    }
}
