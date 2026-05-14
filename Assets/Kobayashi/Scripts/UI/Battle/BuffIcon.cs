using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    [Header("-----éQè∆-----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _turn;
    
    public void SetIconData(BuffData data)
    {
        _icon.sprite = data.Icon;
    }

    public void UpdateTurn(byte turn)
    {
        _turn.text = turn.ToString();
    }
}
