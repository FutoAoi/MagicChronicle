using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    [Header("-----éQè∆-----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _turn;

    private bool _isDisplayCount = true;
    
    public void SetIconData(BuffData data)
    {
        _icon.sprite = data.Icon;
        _isDisplayCount = data.IsDisplayCount;
    }

    public void UpdateTurn(int turn)
    {
        if (!_isDisplayCount)
        {
            _turn.gameObject.SetActive(false);
            return;
        }
        _turn.text = turn.ToString();
    }
}
