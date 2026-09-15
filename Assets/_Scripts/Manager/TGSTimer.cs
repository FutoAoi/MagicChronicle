using UnityEngine;
using UnityEngine.SceneManagement;

public class TGSTimer : MonoBehaviour
{
    public static TGSTimer Instance { get; private set; }

    [Header("タイマー設定")]
    [SerializeField] private float _timeLimit = 540f;      // 制限時間(秒)
    [SerializeField] private float _timeLimit_fin = 600f;
    [SerializeField] private string _nextSceneName = "TitleScene"; // 遷移先シーン名

    private float _currentTime = 0f;
    private bool _isRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!_isRunning) return;

        _currentTime += Time.deltaTime;
    }

    /// <summary>
    /// 現在の時間を確認し、制限時間を超えていたらシーン遷移する
    /// </summary>
    public bool CheckTimeAndTransition()
    {
        if (_currentTime >= _timeLimit)
        {
            _isRunning = false;
            GameManager.Instance.SceneChange(SceneType.ClearScene);
            StopTimer();
            return true;
        }
        return false;
    }

    public bool CheckFinTimeTransition()
    {
        if (_currentTime >= _timeLimit_fin)
        {
            _isRunning = false;
            GameManager.Instance.SceneChange(SceneType.ClearScene);
            StopTimer();
            return true;
        }
        return false;
    }

    /// <summary>
    /// タイマーをリスタートする
    /// </summary>
    public void RestartTimer()
    {
        _currentTime = 0f;
        _isRunning = true;
    }

    /// <summary>
    /// タイマーを一時停止する(必要であれば)
    /// </summary>
    public void StopTimer()
    {
        _currentTime = 0;
        _isRunning = false;
    }
    public float CurrentTime => _currentTime;
}
