using System;
using TMPro;
using UnityEngine;

public class BerserkerManager : MonoBehaviour
{
    [SerializeField] private int angerCount;
    [SerializeField] private TMP_Text _berserkerText;
    [SerializeField] private GameObject _counterObject;
    public int AngerCount => angerCount;

    public event Action<int, int> OnAngerChanged;

    private void OnEnable()
    {
        OnAngerChanged += UIUpdate;
    }

    private void OnDisable()
    {
        OnAngerChanged -= UIUpdate;
    }
    public void Start()
    {
        GameManager.Instance.BerserkerManager = this;
        Initialize();
    }

    public void Initialize(int initialValue = 0)
    {
        if(GameManager.Instance.PlayerType != PlayerType.Berserker)
        {
            _counterObject.SetActive(false);
        }
        int old = angerCount;
        angerCount = initialValue;
        OnAngerChanged?.Invoke(old, angerCount);
    }

    public void AddAnger(int amount)
    {
        if (amount <= 0) return;
        int old = angerCount;
        angerCount += amount;
        OnAngerChanged?.Invoke(old, angerCount);
    }

    public bool CanConsume(int amount) => angerCount >= amount;

    public bool TryConsumeAnger(int amount)
    {
        if (!CanConsume(amount)) return false;
        int old = angerCount;
        angerCount -= amount;
        OnAngerChanged?.Invoke(old, angerCount);
        return true;
    }

    public void DoubleAnger()
    {
        int old = angerCount;
        angerCount *= 2;
        OnAngerChanged?.Invoke(old, angerCount);
    }

    public void UIUpdate(int oldvalue ,int newvalue)
    {
        _berserkerText.text = newvalue.ToString();
    }
}
