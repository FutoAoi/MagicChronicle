using UnityEngine;

public interface IBuff
{
    /// <summary>
    /// バフ実行
    /// </summary>
    /// <param name="character">バフ適応先</param>
    public void Excute(CharacterBase character);
}
