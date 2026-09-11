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
    [SerializeField] private Sprite _cardImage;
    [SerializeField] private Sprite _cardMaskImage;
    [SerializeField] private Sprite _cardBackImage;
    [SerializeField] private Sprite _cardBackgroundImage;
    [SerializeField] private Sprite _cardCostImage;
    [SerializeField] private Sprite _stageSerectIconImage;
    [SerializeField] private Sprite _eventImage;
    [SerializeField] private CardDataBase _cardData;
    [SerializeField] private DeckData _defaultDeck;

    public int PlayerID => _playerID;
    public string PlayerName => _playerName;
    public PlayerType PlayerType => _playerType;
    public int PlayerMaxHp => _playerMaxHp;
    public int PlayerMaxCost => _playerMaxCost;
    public Sprite PlayerSprite => _playerSprite;
    public bool IsPlayable => _isPlayable;
    public string Description => _playerDescription;
    public GameObject PlayerPrefab => _playerPrefab;
    public Sprite CardImage => _cardImage;
    public Sprite CardMaskImage => _cardMaskImage;
    public Sprite CardBackImage => _cardBackImage;
    public Sprite CardBackgroundImage => _cardBackgroundImage;
    public Sprite CardCostImage => _cardCostImage;
    public Sprite StageSerectIconImage => _stageSerectIconImage;
    public Sprite EventImage => _eventImage;
    public CardDataBase CardData => _cardData;
    public DeckData DeckData => _defaultDeck;
}
