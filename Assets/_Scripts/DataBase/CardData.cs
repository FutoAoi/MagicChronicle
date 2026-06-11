using UnityEngine;

[CreateAssetMenu(menuName = "Datas/Card")]
public class CardData : ScriptableObject
{
    [Header("-----カード詳細-----")]
    [SerializeField, Tooltip("カードID")] private int _cardID;
    [SerializeField, Tooltip("カードのレアリティ")] private CardRarity _rarity;
    [SerializeField, Tooltip("カードのタイプ")] private CardType _type;
    [SerializeField, Tooltip("カードの見た目")] private Sprite _cardSprite;
    [SerializeField, Tooltip("魔法陣の見た目")] private Sprite _magicSprite;
    [SerializeField, Tooltip("カードの名前")] private string _name;
    [TextArea(3, 10)]
    [SerializeField, Tooltip("カードの説明")] private string _description;
    [SerializeField, Tooltip("カードのコスト")] private int _cost;
    [SerializeField, Tooltip("最大回数")] private int _maxTimes;
    [SerializeField, Tooltip("霊陣")] private bool _isGhost = false;
    [SerializeField, Tooltip("破棄")] private bool _isDestruction = false;
    [SerializeField, Tooltip("進化できるかのフラグ")] private bool _canEvolution;
    [ShowIf("_canEvolution"),SerializeField, Tooltip("進化先のID")] private int _evolutionID;

    [Header("-----移動効果-----")]
    [SerializeField] private MagicVector[] _displayArrowVector;
    [SerializeReference, SubclassSelector] private IEffect _moveEffect;
    [Header("-----効果-----")]
    [SerializeReference, SubclassSelector] private IEffect[] _effect;

    public int CardID => _cardID;
    public Sprite CardSprite => _cardSprite;
    public Sprite MagicSprite => _magicSprite;
    public string Name => _name;
    public string Description => _description;
    public int Cost => _cost;
    public MagicVector[] DisplayArrowVector => _displayArrowVector;
    public IEffect MoveEffect => _moveEffect;
    public IEffect[] Effect => _effect;
    public int MaxTimes => _maxTimes;
    public CardRarity Rarity => _rarity;
    public CardType Type => _type;
    public bool IsGhost => _isGhost;
    public bool IsDestruction => _isDestruction;
    public bool CanEvolution => _canEvolution;
    public int EvolutionID => _evolutionID;
}