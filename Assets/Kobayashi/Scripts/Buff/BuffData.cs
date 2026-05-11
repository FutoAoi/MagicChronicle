using UnityEngine;
[CreateAssetMenu(menuName = "Datas/Buff")]
public class BuffData : ScriptableObject
{
    [Header("-----Šî–{î•ñ-----")]
    [SerializeField] private BuffType _type;
    [SerializeField] private string _name;
    [TextArea(3, 10)]
    [SerializeField] private string _description;
    [SerializeField] private bool _isDecreaseTurn;
    [SerializeReference, SubclassSelector] private IBuff[] _effect;

    public BuffType Type => _type;
    public string Name => _name;
    public string Description => _description;
    public bool IsDecreaseTurn => _isDecreaseTurn;
    public IBuff[] Effect => _effect;

}
