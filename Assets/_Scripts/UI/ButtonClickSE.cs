using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class ButtonClickSE : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            CriAudioManager.Instance.PlaySe("SE_ButtonClick");
        });
    }
}
