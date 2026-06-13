using UnityEngine;

public class AudioTest : MonoBehaviour
{
    void Start()
    {
        CriAudioManager.Instance.PlayBgm("BGM_Stage1");
    }
}
