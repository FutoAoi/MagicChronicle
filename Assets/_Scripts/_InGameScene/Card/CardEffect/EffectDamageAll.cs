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
        bool bossAttack = false;
        for (int i = 0; i < _attackTargets.Count; i++)
        {
            if (_attackTargets[i].IsBoss)
            {
                if(!bossAttack)
                {
                    _attackTargets[i].Damaged(_damageAmount);
                    bossAttack = true;
                }
                continue;
            }
            _attackTargets[i].Damaged(_damageAmount);
        }
    }
}
