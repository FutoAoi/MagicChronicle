using System.Collections.Generic;
using UnityEngine;

public class EffectDamageEnemy : IEffect
{
    [SerializeField] private int _effectDamege;
    [SerializeField] private bool _canRangeAttack;
    [SerializeField] private bool _isGhost;
    private List<Enemy> _attackTargets = new();
    private int _randomIndex;
    public void OnExcute(AttackMagic magic)
    {
        _attackTargets = GameManager.Instance.StageManager.EnemyList.FindAll(enemy => enemy.IsDead != true);
        if(_attackTargets.Count == 0 ) return;
        if (_canRangeAttack)
        {
            for(int i = 0; i < _attackTargets.Count; i++)
            {
                _attackTargets[i].Damaged(_effectDamege);
                CriAudioManager.Instance.PlaySe("SE_MagicCircleAttackAll");
            }
        }
        else
        {
            _randomIndex = Random.Range(0, _attackTargets.Count);
            _attackTargets[_randomIndex].DamageFromMagicAttacks(_effectDamege,magic.CurrentSlot);
            CriAudioManager.Instance.PlaySe("SE_MagicCircleAttack");
            if(_isGhost)
            {
                CriAudioManager.Instance.PlaySe("SE_MagicCircleAttackStrong");
            }
        }
    }
}
