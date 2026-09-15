using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EffectAngerAttack : EffectUseAnger
{
    [SerializeField] private int _effectDamege;
    [SerializeField] private AttackMagic _magicPrefab;

    private List<Enemy> _attackTargets = new();
    private int _randomIndex;

    protected override void Effect(AttackMagic magic)
    {
        DamageEffect(magic.CurrentSlot);
    }

    private void DamageEffect(Vector2Int origin)
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
                    _attackTargets[i].DamageFromMagicAttacks(_effectDamege, origin, _magicPrefab);
                    bossAttack = true;
                }
                continue;
            }
            _attackTargets[i].DamageFromMagicAttacks(_effectDamege, origin, _magicPrefab);
        }
    }
}
