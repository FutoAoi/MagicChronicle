using System;
using UnityEngine;
/// <summary>
/// カードドロー
/// </summary>
[Serializable]
public class EffectCardDraw : IEffect
{
    [Header("ドロー枚数")]
    [SerializeField] private int _drawNumber = 1;
    public void OnExcute(AttackMagic magic)
    {
        if(GameManager.Instance.CurrentUIManager.TryGetComponent<IBattleUI>(out var battleUI))
        {
            battleUI.ChangeDrawCount(_drawNumber);
        }
    }
}
