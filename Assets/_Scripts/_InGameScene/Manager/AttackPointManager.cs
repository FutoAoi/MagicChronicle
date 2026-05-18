using UnityEngine;
using System.Collections.Generic;

public class AttackPointManager : MonoBehaviour
{
    public List<AttackPointSelectButton> AttackPointButtonList = new List<AttackPointSelectButton>();

    public void ChangeButtonState(int index)
    {
        for (int i = 0; i < AttackPointButtonList.Count; i++)
        {
            if (AttackPointButtonList[i].IsSelect && i != index)
            {
                AttackPointButtonList[i].CancelRegister();
            }
        }
    }

    public void CheckStartAttackPosition()
    {
        AttackPointButtonList[AttackPointButtonList.Count/2].RegisterAttackPosition();
    }
}
