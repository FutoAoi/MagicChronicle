using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EffectAngerAmountAttack : IEffect
{
    private List<Enemy> _attackTargets = new();
    [SerializeField] private AttackMagic _attackMagic;

    public void OnExcute(AttackMagic magic)
    {
        CriAudioManager cri = CriAudioManager.Instance;
        _attackTargets = GameManager.Instance.StageManager.EnemyList.FindAll(enemy => enemy.IsDead != true);
        if (_attackTargets.Count == 0) return;
        cri.PlaySe("SE_MagicCircleAttackAll");
        bool bossAttack = false;
        for (int i = 0; i < _attackTargets.Count; i++)
        {
            if (_attackTargets[i].IsBoss)
            {
                if (!bossAttack)
                {
                    _attackTargets[i].DamageFromMagicAttacks(GameManager.Instance.BerserkerManager.AngerCount, magic.CurrentSlot, _attackMagic);
                    bossAttack = true;
                }
                continue;
            }
            _attackTargets[i].DamageFromMagicAttacks(GameManager.Instance.BerserkerManager.AngerCount, magic.CurrentSlot, _attackMagic);
        }
    }
}
