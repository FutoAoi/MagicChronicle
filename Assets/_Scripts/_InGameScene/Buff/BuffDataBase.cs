using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "DataBase/BuffDataBase")]
public class BuffDataBase : ScriptableObject
{
    public List<BuffData> BuffDatas => _buffDatas;
    [Header("バフデータベース")]
    [SerializeField, Tooltip("バフリスト")] private List<BuffData> _buffDatas = new();

    private Dictionary<BuffType, BuffData> _buffDictionary;

    private void Init()
    {
        if(_buffDictionary == null)
        {
            _buffDictionary = new Dictionary<BuffType, BuffData>();
            foreach(BuffData data in _buffDatas)
            {
                if (!_buffDictionary.ContainsKey(data.Type))
                {
                    _buffDictionary.Add(data.Type, data);
                }
                else
                {

                }
            }
        }
    }
    /// <summary>
    /// バフデータを取得
    /// </summary>
    /// <param name="type">バフの指定</param>
    /// <returns></returns>
    public BuffData GetBuffData(BuffType type)
    {
        Init();

        if(_buffDictionary.TryGetValue(type, out BuffData data))
            return data;

        return null;
    }
}
