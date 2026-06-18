using UnityEngine;

[CreateAssetMenu(menuName = "Datas/Player")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private int _playerID;
    [SerializeField] private string _playerName;
    [SerializeField] private PlayerType _playerType;
    [SerializeField] private int _playerMaxHp;
    [SerializeField] private int _playerMaxCost;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private bool _isPlayable = true;
    [TextArea(3, 10)]
    [SerializeField] private string _playerDescription;
    [SerializeField] private GameObject _playerPrefab;

    public int PlayerID => _playerID;
    public string PlayerName => _playerName;
    public PlayerType PlayerType => _playerType;
    public int PlayerMaxHp => _playerMaxHp;
    public int PlayerMaxCost => _playerMaxCost;
    public Sprite PlayerSprite => _playerSprite;
    public bool IsPlayable => _isPlayable;
    public string Description => _playerDescription;
    public GameObject PlayerPrefab => _playerPrefab;
}
