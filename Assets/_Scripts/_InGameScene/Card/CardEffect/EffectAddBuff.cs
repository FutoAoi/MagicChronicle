using System.Collections.Generic;
using UnityEngine;

public class EffectAddBuff : IEffect
{
    [SerializeField] private BuffType _addBuffType;
    [SerializeField] private bool _canBuffEnemy = false;
    [SerializeField] private bool _isRandom = false;
    private List<Enemy> _attackTargets = new();
    private int _randomIndex;
    public void OnExcute(AttackMagic magic)
    {
        if(_canBuffEnemy)
        {
            _attackTargets = GameManager.Instance.StageManager.EnemyList.FindAll(enemy => enemy.IsDead != true);
            if(_isRandom)
            {
                _randomIndex = Random.Range(0, _attackTargets.Count);
                _attackTargets[_randomIndex].AddBuff(_addBuffType);
            }
            else
            {
                foreach(Enemy enemy in _attackTargets)
                {
                    enemy.AddBuff(_addBuffType);
                }
            }
        }
        else
        {
            GameManager.Instance.Player.AddBuff(_addBuffType);
        }
    }
}
