using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ボリューム設定UI。
/// Master / BGM / SE の3本のSliderをCriAudioManagerにバインドし、
/// PlayerPrefsで設定値を保存・復元します。
///
/// 使い方:
/// 1. Canvas上にSliderを3つ用意し（Min=0, Max=1）、このコンポーネントを
///    空のGameObjectにアタッチしてInspectorから各Sliderを割り当てる。
/// 2. シーンにCriAudioManagerが存在すること（DontDestroyOnLoadで常駐推奨）。
/// 3. このコンポーネントはOnEnable時に保存済み設定を読み込み、
///    Slider操作でリアルタイムに音量を反映＆保存します。
/// </summary>
public class VolumeSettingsUI : MonoBehaviour
{
    [Header("Sliderの参照")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;

    [Header("SEプレビュー再生")]
    [Tooltip("SEスライダーを離したときに確認用SEを鳴らすか")]
    [SerializeField] private bool _playPreviewSe = true;
    [SerializeField] private string _previewSeCueName = "SE_TitleClick";

    // PlayerPrefsキー
    private const string KEY_MASTER = "Volume_Master";
    private const string KEY_BGM = "Volume_Bgm";
    private const string KEY_SE = "Volume_Se";

    private const float DEFAULT_VOLUME = 1.0f;

    private bool _isInitializing;
    private EventTrigger _seSliderEventTrigger;
    private EventTrigger.Entry _sePointerUpEntry;

    private void OnEnable()
    {
        LoadAndApplySavedVolumes();
        BindSliderEvents();
        BindSePreviewPointerUp();
    }

    private void OnDisable()
    {
        UnbindSliderEvents();
        UnbindSePreviewPointerUp();
    }

    /// <summary>保存済みの音量をPlayerPrefsから読み込み、Sliderと実際の音量に反映します。</summary>
    private void LoadAndApplySavedVolumes()
    {
        _isInitializing = true;

        float master = PlayerPrefs.GetFloat(KEY_MASTER, DEFAULT_VOLUME);
        float bgm = PlayerPrefs.GetFloat(KEY_BGM, DEFAULT_VOLUME);
        float se = PlayerPrefs.GetFloat(KEY_SE, DEFAULT_VOLUME);

        if (_masterSlider != null) _masterSlider.SetValueWithoutNotify(master);
        if (_bgmSlider != null) _bgmSlider.SetValueWithoutNotify(bgm);
        if (_seSlider != null) _seSlider.SetValueWithoutNotify(se);

        ApplyVolumesToAudioManager(master, bgm, se);
        _isInitializing = false;
    }

    private void BindSliderEvents()
    {
        if (_masterSlider != null) _masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (_bgmSlider != null) _bgmSlider.onValueChanged.AddListener(OnBgmChanged);
        if (_seSlider != null) _seSlider.onValueChanged.AddListener(OnSeChanged);
    }

    private void UnbindSliderEvents()
    {
        if (_masterSlider != null) _masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        if (_bgmSlider != null) _bgmSlider.onValueChanged.RemoveListener(OnBgmChanged);
        if (_seSlider != null) _seSlider.onValueChanged.RemoveListener(OnSeChanged);
    }

    private void OnMasterChanged(float value)
    {
        if (_isInitializing) return;
        if (CriAudioManager.Instance != null)
            CriAudioManager.Instance.MasterVolume = value;
        PlayerPrefs.SetFloat(KEY_MASTER, value);
        PlayerPrefs.Save();
    }

    private void OnBgmChanged(float value)
    {
        if (_isInitializing) return;
        if (CriAudioManager.Instance != null)
            CriAudioManager.Instance.BgmVolume = value;
        PlayerPrefs.SetFloat(KEY_BGM, value);
        PlayerPrefs.Save();
    }

    private void OnSeChanged(float value)
    {
        // ドラッグ中も呼ばれるので、音量の反映・保存のみ行う（SE再生はPointerUp側で行う）
        if (_isInitializing) return;
        if (CriAudioManager.Instance != null)
            CriAudioManager.Instance.SeVolume = value;
        PlayerPrefs.SetFloat(KEY_SE, value);
        PlayerPrefs.Save();
    }

    /// <summary>SEスライダーのGameObjectにEventTriggerを追加し、PointerUp（操作終了）時だけプレビューSEを鳴らします。</summary>
    private void BindSePreviewPointerUp()
    {
        if (_seSlider == null || !_playPreviewSe) return;

        _seSliderEventTrigger = _seSlider.GetComponent<EventTrigger>();
        if (_seSliderEventTrigger == null)
            _seSliderEventTrigger = _seSlider.gameObject.AddComponent<EventTrigger>();

        _sePointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        _sePointerUpEntry.callback.AddListener(_ => PlaySePreview());
        _seSliderEventTrigger.triggers.Add(_sePointerUpEntry);
    }

    private void UnbindSePreviewPointerUp()
    {
        if (_seSliderEventTrigger != null && _sePointerUpEntry != null)
            _seSliderEventTrigger.triggers.Remove(_sePointerUpEntry);

        _sePointerUpEntry = null;
    }

    private void PlaySePreview()
    {
        if (CriAudioManager.Instance != null)
            CriAudioManager.Instance.PlaySe(_previewSeCueName);
    }

    private void ApplyVolumesToAudioManager(float master, float bgm, float se)
    {
        if (CriAudioManager.Instance == null) return;
        CriAudioManager.Instance.MasterVolume = master;
        //CriAudioManager.Instance.BgmVolume = bgm;
        CriAudioManager.Instance.SeVolume = se;
    }

    /// <summary>設定を初期値にリセットします（「デフォルトに戻す」ボタン等から呼び出し）。</summary>
    public void ResetToDefault()
    {
        PlayerPrefs.DeleteKey(KEY_MASTER);
        PlayerPrefs.DeleteKey(KEY_BGM);
        PlayerPrefs.DeleteKey(KEY_SE);
        PlayerPrefs.Save();
        LoadAndApplySavedVolumes();
    }
}