using System;
using UnityEngine;

/// <summary>
/// •ªŠ„
/// </summary>
[Serializable]
public class EffectSplitAttack : IEffect
{
    [Header("•ªŠ„•ûŒü"), SerializeField] private MagicVector[] _vector;
    public void OnExcute(AttackMagic magic)
    {
        if (magic == null) return;
        MagicObjectPool pool = MagicObjectPool.Instance;
        AttackMagic attack = magic;
        for (int i = 0; i < _vector.Length; i++)
        {
            if (i != 0)
            {
                attack = pool.GetAttackMagic();
            }

            attack.AttackPower = Mathf.Max(1, magic.AttackPower / _vector.Length);

            Vector2Int vector = new();
            switch (_vector[i])
            {
                case MagicVector.UP:
                    vector = new Vector2Int(-1, 0);
                    break;
                case MagicVector.Down:
                    vector = new Vector2Int(1, 0);
                    break;
                case MagicVector.Left:
                    vector = new Vector2Int(0, -1);
                    break;
                case MagicVector.Right:
                    vector = new Vector2Int(0, 1);
                    break;
            }
            if (i == 0)
            {
                attack.ChangeVector(_vector[i]);
            }
            else
            {
                attack.Split(_vector[i], magic.CurrentSlot + vector,
                    magic.gameObject.GetComponent<RectTransform>());
            }
        }
    }
}