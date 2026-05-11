using UnityEngine;

public interface IBuffable
{
    /// <summary>
    /// バフの残り回数を変える
    /// </summary>
    /// <param name="type">バフの指定</param>
    /// <param name="time">足される数</param>
    void AddBuff(BuffType type,int time);
}
