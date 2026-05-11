using TMPro;
using UnityEngine;

public class PlayableCharactorSort : MonoBehaviour
{
    [Header("-----参照-----")]
    [SerializeField, Tooltip("キャラクター")] private GameObject _prefab;
    [SerializeField, Tooltip("説明欄")] private TextMeshProUGUI _description;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}
