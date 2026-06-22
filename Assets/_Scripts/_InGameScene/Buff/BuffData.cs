using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName = "Datas/Buff")]
public class BuffData : ScriptableObject
{
    [Header("-----äÓñ{èÓïÒ-----")]
    [SerializeField] private BuffType _type;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [TextArea(3, 10)]
    [SerializeField] private string _description;
    [SerializeField] private bool _isDecreaseTurn;
    [SerializeField] private bool _isDisplayCount = true;
    [SerializeReference, SubclassSelector] private IBuff[] _effect;
    [SerializeField] private List<DescriptionKeyWord> _keywords = new();

    public BuffType Type => _type;
    public Sprite Icon => _icon;
    public string Name => _name;
    public string Description => _description;
    public bool IsDecreaseTurn => _isDecreaseTurn;
    public bool IsDisplayCount => _isDisplayCount;
    public IBuff[] Effect => _effect;
    public List<DescriptionKeyWord> KeyWords => _keywords;
}
