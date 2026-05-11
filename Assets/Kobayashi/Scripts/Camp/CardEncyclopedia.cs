using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardEncyclopedia : MonoBehaviour
{
    public CardDataBase CardDatas => _cardDatas;

    [Header("-----参照-----")]
    [SerializeField, Tooltip("カードデータベース")] private CardDataBase _cardDatas;
    [SerializeField, Tooltip("カードプレハブ")] private GameObject _prefab;
    [SerializeField, Tooltip("生成場所")] private Transform _parent;

    [Header("-----情報更新先-----")]
    [SerializeField, Tooltip("名前")] private TextMeshProUGUI _name;
    [SerializeField, Tooltip("コスト")] private TextMeshProUGUI _cost;
    [SerializeField, Tooltip("耐久値")] private TextMeshProUGUI _times;
    [SerializeField, Tooltip("イメージ")] private Image _image;
    [SerializeField, Tooltip("説明欄")] private TextMeshProUGUI _description;

    [SerializeField] private bool _usePool = true;

    private List<CardData> _currentList = new();
    private List<CardView> _pool = new();

    private void Awake()
    {
        InitializePool();
    }
    /// <summary>
    /// Hierarchy上のCardViewをPool化
    /// </summary>
    private void InitializePool()
    {
        _pool.Clear();

        for (int i = 0; i < _parent.childCount; i++)
        {
            CardView view = _parent.GetChild(i).GetComponent<CardView>();

            if (view != null)
            {
                //view.gameObject.SetActive(false);
                _pool.Add(view);
            }
        }
    }

    public void UpdateBigCard(int id)
    {
        CardData data = _cardDatas.GetCardData(id);

        _name.text = data.Name;
        _cost.text = data.Cost.ToString();
        _times.text = data.MaxTimes.ToString();
        _description.text = data.Description;
        _image.sprite = data.Sprite;
    }

    /// <summary>
    /// 指定カードの生成
    /// </summary>
    /// <param name="index">データベース内の順番から指定</param>
    public void Generate(CardData data)
    {
        // 重複追加防止
        if (_currentList.Contains(data)) return;

        GameObject obj = Instantiate(_prefab, _parent);
        obj.GetComponent<CardView>().SetCardData(data);
        _currentList.Add(data);
    }
    /// <summary>
    /// 指定のカードの削除
    /// </summary>
    /// <param name="index">データベース内の順番から指定</param>
    public void Clear(CardData data)
    {
        if (_currentList.Contains(data))
        {
            DestroyImmediate(_parent.GetChild(_currentList.IndexOf(data)).gameObject);
            _currentList.Remove(data);
        }
    }
    /// <summary>
    /// カードをすべて生成
    /// </summary>
    public void GenerateAll()
    {
        ClearAll();
        for(int i = 0; i < _cardDatas.Cards.Count; i++)
        {
            Generate(_cardDatas.Cards[i]);
        }
    }
    /// <summary>
    /// カードをすべて削除
    /// </summary>
    public void ClearAll()
    {
        for(int i = _currentList.Count - 1; i >= 0; i--)
        {
            Clear(_currentList[i]);
        }
        _currentList.Clear();
    }

    public CardData GetCardData(int index)
    {
        return _cardDatas.Cards[index];
    }

    /// <summary>
    /// 再描画処理
    /// </summary>
    private void Redraw()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (i < _currentList.Count)
            {
                _pool[i].gameObject.SetActive(true);

                _pool[i].SetCardData(_currentList[i]);

                // 並び順更新
                _pool[i].transform.SetSiblingIndex(i);
            }
            else
            {
                _pool[i].gameObject.SetActive(false);
            }
        }
    }
    public void FilterByCost(int cost)
    {
        _currentList.Clear();

        foreach (CardData card in _cardDatas.Cards)
        {
            if (card.Cost == cost)
            {
                _currentList.Add(card);
            }
        }

        Redraw();
    }

    public void FilterByName(string text)
    {
        _currentList.Clear();

        text = text.ToLower();

        foreach (CardData card in _cardDatas.Cards)
        {
            if (card.Name.ToLower().Contains(text))
            {
                _currentList.Add(card);
            }
        }

        Redraw();
    }
    public void SortByName()
    {
        _currentList.Sort((a, b) => a.Name.CompareTo(b.Name));
        Redraw();
    }

    public void SortByCost()
    {
        _currentList.Sort((a, b) => a.Cost.CompareTo(b.Cost));
        Redraw();
    }
}

