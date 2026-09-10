using System.Collections.Generic;
using UnityEngine;

public class EffectAngerAttack : EffectUseAnger
{
    [SerializeField] private int _effectDamege;

    private List<Enemy> _attackTargets = new();
    private int _randomIndex;

    protected override void Effect(AttackMagic magic)
    {
        DamageEffect(magic);
    }

    private void DamageEffect(AttackMagic magic)
    {
        CriAudioManager cri = CriAudioManager.Instance;
        _attackTargets = GameManager.Instance.StageManager.EnemyList.FindAll(enemy => enemy.IsDead != true);
        if (_attackTargets.Count == 0) return;
        cri.PlaySe("SE_MagicCircleAttackAll");
        for (int i = 0; i < _attackTargets.Count; i++)
        {
            _attackTargets[i].Damaged(_effectDamege);
        }
    }
}
