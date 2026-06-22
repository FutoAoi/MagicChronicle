using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "DataBase/KeyWord")]
public class KeywordDataBase : ScriptableObject
{
#if UNITY_EDITOR
    public static KeywordDataBase Instance { get; private set; }
    void OnEnable() => Instance = this;
#endif
    [SerializeField] List<KeywordData> _keywords = new();
    public List<KeywordData> Keywords => _keywords;
    private Dictionary<DescriptionKeyWord, KeywordData> _keywordsDictionary;

    public KeywordData GetDataEditor(DescriptionKeyWord type)
    {
        return Keywords.Find(k => k.Type == type);
    }

    void Initialize()
    {
        if (_keywordsDictionary == null)
        {
            _keywordsDictionary = new Dictionary<DescriptionKeyWord, KeywordData>();
            foreach (var data in _keywords)
            {
                if (!_keywordsDictionary.ContainsKey(data.Type))
                {
                    _keywordsDictionary.Add(data.Type, data);
                }
            }
        }
    }
    public KeywordData GetKeyWordData(DescriptionKeyWord type)
    {
        Initialize();
        if(_keywordsDictionary.TryGetValue(type, out var data))
        {
            return data;
        }
        return null;
    }
}
