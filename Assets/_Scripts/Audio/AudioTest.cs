using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] string BGM;
    void Start()
    {
        CriAudioManager.Instance.PlayBgm(BGM);
    }
}
