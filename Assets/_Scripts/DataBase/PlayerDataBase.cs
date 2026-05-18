using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataBase/PlayerDataBase")]
public class PlayerDataBase : ScriptableObject
{
    [Header("プレイヤーデータベース")]
    [SerializeField, Tooltip("プレイヤーリスト")] private List<PlayerData> _players = new();

    private Dictionary<int, PlayerData> _playerDictionary;

    /// <summary>
    /// Dicrionaryに登録
    /// </summary>
    private void Initialized()
    {
        if(_playerDictionary != null)
        {
            _playerDictionary = new();
            foreach(var player in _players)
            {
                if (!_playerDictionary.ContainsKey(player.PlayerID))
                {
                    _playerDictionary.Add(player.PlayerID, player);
                }
                else
                {
                    Debug.LogWarning($"重複したキーがあります:{player.PlayerID}");
                }
            }
        }
    }

    /// <summary>
    /// プレイヤーのIDを入れるとプレイヤーデータを取得できる
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public PlayerData GetPlayerData(int ID)
    {
        Initialized();
        if(_playerDictionary.TryGetValue(ID, out PlayerData playerData))
        {
            return playerData;
        }
        Debug.LogWarning($"ID{ID}の敵が見つかりません");
        return null;
    }
}
