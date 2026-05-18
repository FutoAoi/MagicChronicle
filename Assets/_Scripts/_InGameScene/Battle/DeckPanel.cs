using System.Collections.Generic;
using UnityEngine;

public class DeckPanel : MonoBehaviour
{
    [Header("各場所のパネル")]
    [SerializeField, Tooltip("山札")] private GameObject _deckPanel;
    [SerializeField, Tooltip("捨て札")] private GameObject _discardPanel;
    [SerializeField, Tooltip("除外札")] private GameObject _removePanel;

    [Header("並べるプレハブ")]
    [SerializeField, Tooltip("デッキ確認用プレハブ")] private GameObject _cardPrefab;

    GameManager _gameManager;
    UIManager_Battle _uiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameManager.Instance;
        if(_gameManager.CurrentUIManager.TryGetComponent(out UIManager_Battle ui))
        {
            _uiManager = ui;
        }
    }
    /// <summary>
    /// デッキの現状をパネルに反映
    /// </summary>
    public void DisplayDeckContents(InGameDeckType deckType)
    {
        List<int> ids = new List<int>();
        switch(deckType)
        {
            case InGameDeckType.Deck:

                break;
            case InGameDeckType.Discard:
                break;
            case InGameDeckType.Remove:
                break;
            default:
                break;
        }
    }
}
