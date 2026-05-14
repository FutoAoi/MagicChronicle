using UnityEngine;

public class ComboBuff : IBuff
{
    public void Excute(CharacterBase character)
    {
        character.AddBuff(BuffType.Combo, 255);
    }
}
