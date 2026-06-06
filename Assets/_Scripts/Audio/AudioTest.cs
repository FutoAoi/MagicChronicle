using UnityEngine;

public class AudioTest : MonoBehaviour
{

    [SerializeField] private AudioManager manager;
    [SerializeField] private AudioSource audioSource;
    void Start()
    {
       manager = AudioManager.Instance;
       manager.PlayBGM("InGame");
    }
}
