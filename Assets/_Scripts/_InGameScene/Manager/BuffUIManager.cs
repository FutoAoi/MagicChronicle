using System.Collections.Generic;
using UnityEngine;

public class BuffUIManager : MonoBehaviour
{
    [Header("-----éQè∆-----")]
    [SerializeField] private GameObject _buffIconPrefab;
    [SerializeField] private RectTransform _iconParent;

    private Dictionary<BuffType, BuffIcon> _iconDictionary;
    private GameManager _gameManager;
    private bool _isSetting = false;

    private void Start()
    {
        if (!_isSetting)
            Init();
    }
    private void Init()
    {
        if (_iconDictionary != null) return;

        _gameManager = GameManager.Instance;
        List<BuffData> buffDatas = _gameManager.BuffDataBase.BuffDatas;
        _iconDictionary = new Dictionary<BuffType, BuffIcon>();
        for (byte i = 0; i < buffDatas.Count; i++)
        {
            GameObject icon = Instantiate(_buffIconPrefab, _iconParent);
            BuffIcon buffIcon = icon.GetComponent<BuffIcon>();
            _iconDictionary.Add(buffDatas[i].Type, buffIcon);
            buffIcon.SetIconData(buffDatas[i]);
            icon.SetActive(false);
        }
        _isSetting = true;
    }
    public void DisplayBuff(BuffType type,int turn)
    {
        if(!_isSetting) Init();

        if (_iconDictionary.TryGetValue(type, out BuffIcon icon))
        {
            icon.gameObject.SetActive(true);
            SetTurn(type, turn);
            Debug.Log(type);
        }
    }

    public void SetTurn(BuffType type,int turn)
    {
        if (_iconDictionary.TryGetValue(type, out BuffIcon icon))
        {
            if (!icon.gameObject.activeSelf) return;

            icon.UpdateTurn(turn);
        }
    }

    public void FalseIcon(BuffType type)
    {
        if (!_isSetting) Init();
        if (_iconDictionary.TryGetValue(type, out BuffIcon icon))
        {
            if (icon.gameObject.activeSelf)
                icon.gameObject.SetActive(false);
        }
    }
}
