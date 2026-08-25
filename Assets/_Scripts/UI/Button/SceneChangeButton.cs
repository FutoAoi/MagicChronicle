using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private SceneType _sceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(SceneChange);
    }
    private void SceneChange()
    {
        CriAudioManager.Instance.PlaySe("SE_ButtonCharaOK");
        if ( _sceneName == SceneType.InGameScene && Platform.IsAndroid)
        {
            GameManager.Instance.SceneChange(SceneType.InGameScene_Android);
            return;
        }
        GameManager.Instance.SceneChange(_sceneName);
    }
}
