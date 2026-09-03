using System.Collections.Generic;
using UnityEngine;

public class EffectDamageEnemy : IEffect,IGhostOriginEffect
{
    [SerializeField] private int _effectDamege;
    [SerializeField] private bool _canRangeAttack;
    [SerializeField] private bool _isGhost;
    [SerializeField, Tooltip("”ñ—ìw‚ÌUŒ‚")] private AttackMagic _magicPrefab;
    [SerializeField, Tooltip("—ìw‚ÌUŒ‚")] private AttackMagic _ghostMagicPrefab;
    private List<Enemy> _attackTargets = new();
    private int _randomIndex;
    public void OnExcute(AttackMagic magic)
    {
        DamageEffect(magic.CurrentSlot);
    }

    public void OnExcute(Vector2Int originSlot)
    {
        DamageEffect(originSlot);
    }

    private void DamageEffect(Vector2Int origin)
    {
        CriAudioManager cri = CriAudioManager.Instance;
        _attackTargets = GameManager.Instance.StageManager.EnemyList.FindAll(enemy => enemy.IsDead != true);
        if (_attackTargets.Count == 0) return;
        if (_canRangeAttack)
        {
            for (int i = 0; i < _attackTargets.Count; i++)
            {
                _attackTargets[i].Damaged(_effectDamege);
                cri.PlaySe("SE_MagicCircleAttackAll");
            }
        }
        else
        {
            AttackMagic prefab = _isGhost && _ghostMagicPrefab != null ? _ghostMagicPrefab : _magicPrefab;
            _randomIndex = Random.Range(0, _attackTargets.Count);
            _attackTargets[_randomIndex].DamageFromMagicAttacks(_effectDamege,origin,prefab);
            cri.PlaySe("SE_MagicCircleAttack");
            if (_isGhost)
            {
                cri.PlaySe("SE_MagicCircleAttackStrong");
            }
        }
    }
}
