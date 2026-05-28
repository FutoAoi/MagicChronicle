using UnityEngine;

public class CostPlusBuff : IBuff
{
    public void Excute(CharacterBase character = null, AttackMagic magic = null)
    {
        GameManager.Instance.CurrentUIManager.UpdateCostUI();
    }
}
