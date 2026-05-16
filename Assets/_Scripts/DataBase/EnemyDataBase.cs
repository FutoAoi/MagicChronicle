using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataBase/EnemyDataBase")]
public class EnemyDataBase : ScriptableObject
{
    [Header("エネミーデータベース")]
    [SerializeField, Tooltip("エネミーリスト")] private List<EnemyData> _enemys = new();

    private Dictionary<int, EnemyData> _enemyDictionary;

    /// <summary>
    /// Dictionaryに登録
    /// </summary>
    private void Initialize()
    {
        if (_enemyDictionary == null)
        {
            _enemyDictionary = new();
            foreach (var enemy in _enemys)
            {
                if (!_enemyDictionary.ContainsKey(enemy.EnemyID))
                {
                    _enemyDictionary.Add(enemy.EnemyID, enemy);
                }
                else
                {
                    Debug.LogWarning($"重複したキーがあります:{enemy.EnemyID}");
                }
            }
        }
    }

    /// <summary>
    /// エネミーのIDを入れるとエネミーデータを取得できる
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public EnemyData GetEnemyData(int ID)
    {
        Initialize();
        if (_enemyDictionary.TryGetValue(ID, out EnemyData enemyData))
        {
            return enemyData;
        }
        Debug.LogWarning($"ID{ID}の敵が見つかりません");
        return null;
    }
}
