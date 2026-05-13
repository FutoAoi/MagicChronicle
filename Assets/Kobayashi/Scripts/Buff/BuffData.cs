using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Datas/Buff")]
public class BuffData : ScriptableObject
{
    [Header("-----Šî–{î•ñ-----")]
    [SerializeField] private BuffType _type;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [TextArea(3, 10)]
    [SerializeField] private string _description;
    [SerializeField] private bool _isDecreaseTurn;
    [SerializeReference, SubclassSelector] private IBuff[] _effect;

    public BuffType Type => _type;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public bool IsDecreaseTurn => _isDecreaseTurn;
    public IBuff[] Effect => _effect;

}
