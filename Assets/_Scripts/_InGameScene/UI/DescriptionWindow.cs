using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionWindow : MonoBehaviour
{
    [Header("-----éQè∆-----")]
    [SerializeField] private Image _img;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    
    public void SetBuffWindow(BuffData data)
    {
        _img.sprite = data.Icon;
        _name.text = data.Name;
        _description.text = data.Description;
    }

    public void SetKeyWordWindow(KeywordData data)
    {
        _name.text = data.KeyName;
        _description.text = data.Description;
    }
}
