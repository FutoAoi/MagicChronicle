using System.Collections.Generic;
using UnityEngine;

public class BuffUIManager : MonoBehaviour
{
    [Header("-----éQè∆-----")]
    [SerializeField] private GameObject _buffIconPrefab;
    [SerializeField] private RectTransform _iconParent;

    private Dictionary<BuffType, BuffIcon> _iconDictionary;
    //private List<GameObject> _buffIcons = new List<GameObject>();

    private void Start()
    {
        _iconDictionary = new Dictionary<BuffType, BuffIcon>();
        for(byte i = 0; i < (byte)BuffType.End; i++)
        {
            GameObject icon = Instantiate(_buffIconPrefab, _iconParent);
            //_buffIcons.Add(icon);
            BuffIcon buffIcon = icon.GetComponent<BuffIcon>();
            _iconDictionary.Add((BuffType)i, buffIcon);
            icon.SetActive(false);
        }
    }
    public void DisplayBuff(BuffType type,byte turn)
    {
        if(_iconDictionary.TryGetValue(type, out BuffIcon icon))
        {
            icon.gameObject.SetActive(true);
            SetTurn(type, turn);
        }
    }

    public void SetTurn(BuffType type,byte turn)
    {
        if (_iconDictionary.TryGetValue(type, out BuffIcon icon))
        {
            if (!icon.gameObject.activeSelf) return;

            icon.UpdateTurn(turn);
        }
    }

    public void FalseIcon(BuffType type)
    {
        if (_iconDictionary.TryGetValue(type, out BuffIcon icon))
        {
            if (icon.gameObject.activeSelf)
                icon.gameObject.SetActive(false);
        }
    }
}
