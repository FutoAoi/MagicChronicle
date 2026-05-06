using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardEncyclopedia : MonoBehaviour
{
    public CardDataBase CardDatas => _cardDatas;

    [Header("-----参照-----")]
    [SerializeField, Tooltip("カードデータベース")] private CardDataBase _cardDatas;
    [SerializeField, Tooltip("デカ表示カード")] private GameObject _bigCard;
    [SerializeField, Tooltip("説明欄")] private TextMeshProUGUI _description;
    [SerializeField, Tooltip("カードプレハブ")] private GameObject _prefab;
    [SerializeField, Tooltip("生成場所")] private Transform _parent;

    [SerializeField] private bool _usePool = true;

    private List<CardData> _currentList = new();
    private List<CardView> _pool = new();
    private int _poolSize;

    private void Awake()
    {
        _poolSize = _cardDatas.Cards.Count;
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_prefab, _parent);
            obj.SetActive(false);
            _pool.Add(obj.GetComponent<CardView>());
        }

    #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            _usePool = false;
        }
    #endif
    }
    /// <summary>
    /// 指定カードの生成
    /// </summary>
    /// <param name="index">データベース内の順番から指定</param>
    public void Generate(int index)
    {
        _currentList.Add(_cardDatas.Cards[index]);
        Redraw();
    }
    /// <summary>
    /// 指定のカードの削除
    /// </summary>
    /// <param name="index">データベース内の順番から指定</param>
    public void Clear(int index)
    {
        int id = _cardDatas.Cards[index].CardID;

        foreach (CardView view in _pool)
        {
            if (view != null && view.ID == id)
            {
                view.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// カードをすべて生成
    /// </summary>
    public void GenerateAll()
    {
        _currentList = new List<CardData>(_cardDatas.Cards);
        Redraw();
    }
    /// <summary>
    /// カードをすべて削除
    /// </summary>
    public void ClearAll()
    {
    #if UNITY_EDITOR
        if (!_usePool)
        {
            for (int i = _parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_parent.GetChild(i).gameObject);
            }
            return;
        }
    #endif

        foreach (var view in _pool)
        {
            if (view != null)
            {
                view.gameObject.SetActive(false);
            }
        }
    }

    public void RefreshAll()
    {
        _currentList = new List<CardData>(_cardDatas.Cards);
        Redraw();
    }

    private void RedrawRuntime()
    {
        // 壊れた参照除去
        _pool.RemoveAll(v => v == null);

        // 足りない分生成
        while (_pool.Count < _currentList.Count)
        {
            GameObject obj = Instantiate(_prefab, _parent);
            obj.SetActive(false);
            _pool.Add(obj.GetComponent<CardView>());
        }

        // 更新
        for (int i = 0; i < _pool.Count; i++)
        {
            if (i < _currentList.Count)
            {
                _pool[i].gameObject.SetActive(true);
                _pool[i].SetCardData(_currentList[i]);
            }
            else
            {
                _pool[i].gameObject.SetActive(false);
            }
        }
    }
    private void RedrawEditor()
    {
        // 全削除
        for (int i = _parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_parent.GetChild(i).gameObject);
        }

        // 生成
        for (int i = 0; i < _currentList.Count; i++)
        {
            GameObject obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(_prefab);
            obj.transform.SetParent(_parent, false);

            obj.GetComponent<CardView>().SetCardData(_currentList[i]);
        }
    }
    /// <summary>
    /// 再描画処理
    /// </summary>
    private void Redraw()
    {
    #if UNITY_EDITOR
        if (!_usePool)
        {
            RedrawEditor();
            return;
        }
    #endif

        RedrawRuntime();
    }
    public void FilterByCost(int cost)
    {
        _currentList = _cardDatas.Cards.FindAll(c => c.Cost == cost);
        Redraw();
    }

    public void FilterByName(string text)
    {
        _currentList = _cardDatas.Cards.FindAll(c =>
            c.Name.ToLower().Contains(text.ToLower())
        );

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

