using System.Collections.Generic;
using UnityEngine;

public class EffectAngerAmountAttack : IEffect
{
    private List<Enemy> _attackTargets = new();

    public void OnExcute(AttackMagic magic)
    {
        CriAudioManager cri = CriAudioManager.Instance;
        _attackTargets = GameManager.Instance.StageManager.EnemyList.FindAll(enemy => enemy.IsDead != true);
        if (_attackTargets.Count == 0) return;
        cri.PlaySe("SE_MagicCircleAttackAll");
        for (int i = 0; i < _attackTargets.Count; i++)
        {
            _attackTargets[i].Damaged(GameManager.Instance.BerserkerManager.AngerCount);
        }
    }
}
