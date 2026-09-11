using UnityEngine;

public class EffectAngerMagic : IEffect
{
    public void OnExcute(AttackMagic magic)
    {
        magic.AttackPower += GameManager.Instance.BerserkerManager.AngerCount;
        CriAudioManager.Instance.PlaySe("SE_MagicCircleMagicPowerUp");
    }
}
